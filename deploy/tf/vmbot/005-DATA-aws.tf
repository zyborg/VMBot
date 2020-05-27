
######################################################################
## AWS Common Data Sources
######################################################################

data "aws_caller_identity" "current" {
    ## No params, but exposes the following attributes:
    ##  * account_id
    ##  * arn
    ##  * user_id
}