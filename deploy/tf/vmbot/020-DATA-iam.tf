
locals {
    ssmparams_prefix_wo_slashes = replace(
        replace(local.ssmparams_prefix, "/^//", ""), "//$/", "")
}

data "aws_iam_policy_document" "lambda_assume_role" {
    policy_id = "Lambda-Assume-Role"
    statement {
        effect  = "Allow"
        actions = ["sts:AssumeRole"]
        principals {
            type        = "Service"
            identifiers = ["lambda.amazonaws.com"]
        }
    }
}

## Define custom policy that allows:
##  * reading SSM params by path prefix for config data
##  * reading tags which we use to specify certain params
##    for the instance, such as connection type 
data "aws_iam_policy_document" "vmbot" {
    statement {
        sid       = "SSMReadParamsByPath"
        effect    = "Allow"
        actions   = [
            "ssm:GetParametersByPath"
        ]
        resources = [
            "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/${local.ssmparams_prefix_wo_slashes}/",
            "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/${local.ssmparams_prefix_wo_slashes}/*",
        ]
    }

    ## Allow reading EC2 instance details including tags
    statement {
        sid       = "EC2Read"
        effect    = "Allow"
        actions   = [
            "ec2:DescribeInstances",
            "ec2:DescribeTags",
        ]
        resources = ["*"]
    }

    ## Allow R53 changes
    statement {
        sid     = "R53Change"
        effect  = "Allow"
        actions = [
            "route53:ListResourceRecordSets",
            "route53:ChangeResourceRecordSets",
        ]
        resources = ["*"]
    }

    ## FUTURE?
    #statement {
    #    sid       = "SESSendEmail"
    #    effect    = "Allow"
    #    actions   = [
    #        "ses:SendEmail",
    #        "ses:SendRawEmail",
    #    ]
    #    resources = ["*"]
    #}
}
