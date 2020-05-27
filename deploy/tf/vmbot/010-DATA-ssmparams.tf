
variable "_ssmparam_prefix" {
    description = <<-DESC
        Override the prefix path to select SSM Parameters to be included
        in the Lambda's configuration, but default this will resolve to
        `/vmbot/`.
        DESC
    type    = string
    default = null
}

locals {
    ssmparams_prefix = (var._ssmparam_prefix != null
        ? var._ssmparam_prefix
        : "/vmbot/")
}
