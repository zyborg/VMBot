
variable "_lambda_role_name" {
    default = "VMBot_Lambda_Role"
}

variable "_lambda_role_policies" {
    description = <<-DESC
        Defines one or more IAM Policy ARNs that will be attached to the Lambda
        exection role.  This should include one of the AWS-managed policies for
        Lambda execution (or the permissions-equivalent of one of these) or the
        Lambda will not be able to execute.
        DESC

    ## Attach AWS-managed policy for basic Lambda execution, essentially ability
    ## to create and write to CloudWatch Logs and to manage ENI interfaces in VPC
    default = [
        ## Access to write logs
        # "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
        ## Access to write logs and manage ENIs on EC2 instances
        "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
    ]
}

######################################################################
## IAM Roles
######################################################################

resource "aws_iam_role" "vmbot_lambda" {
    name        = var._lambda_role_name
    description = "Bestows permissions to the VMBot to perform EC2 inquiries and SES emailing."

    assume_role_policy = data.aws_iam_policy_document.lambda_assume_role.json

    tags = {
        (var.tf_tag_name) = var.tf_tag_value
    }
}
## Define role inline policy for the primary features of the Lambda function
## (reading EC2 info and tags, reading SSM Params, etc.)
resource "aws_iam_role_policy" "vmbot" {
    name   = "VMBot-Policy"
    role   = aws_iam_role.vmbot_lambda.name
    policy = data.aws_iam_policy_document.vmbot.json
}
## Here we attach an additional policies to the Lambda role including
## any required policies needed by a Lambda function to invoke, such as
## one of the AWS-managed policies and any additional policies the user
## may want to give to the Lambda function such as reading from an S3
## bucket to fetch its notification templates
resource "aws_iam_role_policy_attachment" "vmbot_lambda_execution" {
    count = length(var._lambda_role_policies)

    role       = aws_iam_role.vmbot_lambda.name
    ## Access to write logs and manage ENIs on EC2 instances
    policy_arn = var._lambda_role_policies[count.index]
}
