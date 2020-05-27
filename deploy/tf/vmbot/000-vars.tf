
## These two vars should be used in concert to add a tag
## (or equivalent) to any resource created by this module:
## e.g.
##    tags = {
##        "${var.tf_tag_name}" = var.tf_tag_value
##    }

variable "aws_region" {
}

variable "tf_tag_name" {
    description = <<-DESC
        Name of a tag used to mark all resources created by this module.
        DESC
}

variable "tf_tag_value" {
    description = <<-DESC
        The value of a tag used to mark all resources created
        by this module with the name `var.tf.tag_name`.
        DESC
}

variable "vmbot_release" {
    description = <<-DESC
        Specify the version of the VMBot Lambda package binary to install.
        You can provide the special tag `latest` to use the latest release
        or you can provide a specific version tag to use that version or
        you can provide an null value to specify a path to your own package
        in the `vmbot_package` variable.
        
        The default is `latest`.
        DESC
    type    = string
    default = "latest"    
}

variable "vmbot_package" {
    description = <<-DESC
        If the variable `vmbot_release` is the null value, this variable
        should be the local path to the Lambda package ZIP fileto be
        deployed.  Otherwise this variable is ignored.

        The default is null.
        DESC
    type    = string
    default = null
}
variable "deploy_s3_bucket" {
    description = "The name of an S3 bucket to publish the Lambda package for deployment."
}
variable "deploy_s3_key" {
    description = "The key path in the S3 bucket to publish the Lambda package for deployment."
}

variable "lambda_subnet_ids" {
    description = <<-DESC
        Specify a list of Subnet IDs to which the
        Lambda endpoint will be deployed.
        DESC
    type = list
}
variable "lambda_security_group_ids" {
    description = <<-DESC
        Specify a list of Security Group IDs which
        will be applied to the Lambda endpoint.
        DESC
    type = list
}
variable "lambda_env_vars" {
    description = "You should specify a map of environment variables to adjust the configuration."
    type        = map
    default     = {
        ## Need to have at least one value in the Lambda ENV configuration
        ## In practice this should never be an issue because we'll need at
        ## least one config value to enable either email or Slack messages
        NOOP = ""
    }
}
