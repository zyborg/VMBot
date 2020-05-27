

######################################################################
## SSM Params
######################################################################

## --- Future ---

# resource "aws_ssm_parameter" "vmbot" {
#     lifecycle {
#         ignore_changes = [value, type]
#     }

#     name  = "${local.ssmparams_prefix}dbpass"
#     type  = "String"
#     value = var.lambda_db_password
# }
