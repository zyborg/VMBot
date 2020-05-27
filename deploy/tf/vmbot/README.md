# README - VMBot Deployment Terraform Module

This Terraform (TF) module can be used to deploy the VMBot Lambda function and supporting
resources.

## Primary Parameters

The following _primary_ parameters are used to control the common deployment scenarios.

* **`tf_tag_name`** - (Required) Any resource that is created by this module and supports Tags
  will have a tag created with this name.  This is used to identify any such resources in an AWS
  environment that are managed by this module.  The name is not significant, it's just useful to
  the operator.

* **`tf_tag_value`** - The value for the Tag created by `tf_tag_name` parameter.  The value is
  not significant, it's just useful to the operator, for example to distinguish it from other
  resources or other instances of the same module invocation.  One option is to use the name of
  the Terraform configuration that invoked the module to help identify the origin of such
  resources.

* **`vmbot_package`** - (Required) This should be the local file path to the VMBot Lambda
  package ZIP file to be deployed.

* **`deploy_s3_bucket`** - (Required) The S3 Bucket where the VMBot Lambda package ZIP will
  be copied to for deployment.

* **`deploy_s3_key`** - (Required) The S3 Key path where the VMBot Lambda package ZIP will
  be copied to for deployment.

* **`lambda_vpc_subnet_ids`** - (Optional) To deploy the Lambda function inside a
  VPC, provide a list of subnet IDs here.  You **must** also specify the
  `lambda_vpc_security_group_ids` parameter.

* **`lambda_vpc_security_group_ids`** - (Optional) To deploy the Lambda function inside a
  VPC, provide a list of security group IDs here.  You **must** also specify the
  `lambda_vpc_subnet_ids` parameter.

* **`lambda_env_vars`** - (Optional) This can be used to configure the runtime configuration
  of VMBot by specifying environment variables defined during deployment.  (FUTURE)
