
variable "_lambda_package" {
    description = <<-DESC
        Defines the local source on disk for the VMBot Lambda package ZIP file.
        By default we pull from the build output folder for the VMBot source
        path in the adjacent source code tree structure.
        DESC
    type    = string
    default = null
}
variable "_deploy_s3_storage_class" {
    default = "REDUCED_REDUNDANCY"
}
variable "_lambda_timeout" {
    default = "30"
}

locals {
    lambda_package = (var._lambda_package == null
        ? "${path.module}/res/Zyborg.VMBot.zip"
        : var._lambda_package)
    
    ## If both VPC Subnets and SGs are provided, setup to deploy this to a VPC
    vpc_config = ((var.lambda_vpc_subnet_ids != null && var.lambda_vpc_security_group_ids != null)
        ? { vpc = true }
        : { })
}

######################################################################
## Lambda Function and Related Resources
######################################################################

resource "aws_s3_bucket_object" "lambda_package" {
    bucket        = var.deploy_s3_bucket
    key           = var.deploy_s3_key
    storage_class = var._deploy_s3_storage_class

    ## The etag is *more-or-less* an MD5 hash of the file, which can be compared
    ## to the S3 computed etag to trigger an update if the two hashes don't match
    etag   = filemd5(local.lambda_package)
    ## Source path is relative to the "res" sub-dir of the TF config root
    source = local.lambda_package

    tags = {
        (var.tf_tag_name) = var.tf_tag_value
    }
}

## Deploy the Lambda Function
resource "aws_lambda_function" "vmbot" {
    depends_on = [aws_s3_bucket_object.lambda_package]

    function_name = "vmbot"
    description   = "VMBot - multi-function automation helper routines."
    s3_bucket     = aws_s3_bucket_object.lambda_package.bucket
    s3_key        = aws_s3_bucket_object.lambda_package.key

    source_code_hash = filebase64sha256(local.lambda_package)

    role    = aws_iam_role.vmbot_lambda.arn
    handler = "Zyborg.VMBot::Zyborg.VMBot.Function::FunctionHandler"
    runtime = "dotnetcore3.1"

    dynamic "vpc_config" {
        for_each = local.vpc_config
        content {
            subnet_ids         = var.lambda_vpc_subnet_ids
            security_group_ids = var.lambda_vpc_security_group_ids
        }
    }

    timeout = var._lambda_timeout
    tags = {
        (var.tf_tag_name) = var.tf_tag_value
    }
    environment {
        variables = var.lambda_env_vars
    }
}

output "vmbot_lambda_arn" {
    value = aws_lambda_function.vmbot.arn
}
