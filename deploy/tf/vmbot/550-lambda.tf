
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
        ? "${path.module}/../../../src/Zyborg.VMBot/bin/Release/netcoreapp3.1/Zyborg.VMBot.zip"
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
    # source_code_hash = "${data.aws_s3_bucket_object.lambda_package.metadata["Terraform_base64sha256"]}"

    role    = aws_iam_role.vmbot_lambda.arn
    handler = "VMBot::Zyborg.VMBot.Function::FunctionHandler"
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


# ## We setup 2 CloudWatch Events Rules on a scheduled basis
# ##  * First, to generate a credential report
# ##  * Second, to process the credential report
# resource "aws_cloudwatch_event_rule" "generate_cred_report" {
#     name        = "vmbot_GenerateCredReport"
#     description = "Invokes vmbot to generate an IAM Credential Report."
#     is_enabled  = true
#     schedule_expression = "${var._cwevents_generate_cred_report_schedule}"
# }
# resource "aws_lambda_permission" "cwevents" {
#     statement_id  = "CWEvents-Invoke-vmbot-Generate"
#     principal     = "events.amazonaws.com"
#     source_arn    = "${aws_cloudwatch_event_rule.generate_cred_report.arn}"
#     action        = "lambda:InvokeFunction"
#     function_name = "${aws_lambda_function.vmbot.function_name}"
# }
# resource "aws_cloudwatch_event_target" "generate_cred_report" {
#     rule      = aws_cloudwatch_event_rule.generate_cred_report.name
#     arn       = aws_lambda_function.vmbot.arn
#     input     = "\"generate-report\""
#     target_id = "Generate-Credential-Report"
# }
# resource "aws_cloudwatch_event_rule" "process_cred_report" {
#     name        = "vmbot_ProcessCredReport"
#     description = "Invokes vmbot to process notifications against an IAM Credential Report."
#     is_enabled  = true
#     schedule_expression = var._cwevents_process_cred_report_schedule
# }
# resource "aws_lambda_permission" "cwevents_process_cred_report" {
#     statement_id  = "CWEvents-Invoke-vmbot-Process"
#     principal     = "events.amazonaws.com"
#     source_arn    = aws_cloudwatch_event_rule.process_cred_report.arn
#     action        = "lambda:InvokeFunction"
#     function_name = aws_lambda_function.vmbot.function_name
# }
# resource "aws_cloudwatch_event_target" "process_cred_report" {
#     rule      = aws_cloudwatch_event_rule.process_cred_report.name
#     arn       = aws_lambda_function.vmbot.arn
#     input     = "null"
#     target_id = "Process-Credential-Report"
# }
