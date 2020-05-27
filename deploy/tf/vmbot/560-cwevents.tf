
locals {
    cwevents_ec2_trigger_states = [
        ## The only state that triggers adding/updating
        ## the Route53 mapping to the EC2 instance
        "running",

        ## The various states that trigger removing
        ## existing Route53 mapping to the EC2 instances
        "rebooting",
        "stopping",
        "shutting-down",
        #"stopped",
        #"terminated",
    ]
}

######################################################################
## CloudWatch Events Rules and Related Resources
######################################################################

## EC2 Lifecycles:
##  https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-instance-lifecycle.html

## Define CWEvents rule for capturing EC2 lifecycle events for specific states
resource "aws_cloudwatch_event_rule" "ec2states" {
    name          = "EC2States-Invoke-VMBot"
    description   = "Notifies VMBot when instances change state to perform various automations."
    event_pattern = <<-PATTERN
        {
            "source": [
                "aws.ec2"
            ],
            "detail-type": [
                "EC2 Instance State-change Notification"
            ],
            "detail": {
                "state": [
                    "${join("\",\"", local.cwevents_ec2_trigger_states)}"
                ]
            }
        }
        PATTERN
}

## Grant permission to CWEvents for invoking Lambda Function
resource "aws_lambda_permission" "ec2states-to-vmbot" {
    statement_id  = "AllowCWEventsInvokeForEc2"
    principal     = "events.amazonaws.com"
    source_arn    = aws_cloudwatch_event_rule.ec2states.arn
    action        = "lambda:InvokeFunction"
    function_name = aws_lambda_function.vmbot.function_name
}

## Map rule to Lambda Function
resource "aws_cloudwatch_event_target" "ec2states-to-vmbot" {
    rule      = aws_cloudwatch_event_rule.ec2states.name  
    arn       = aws_lambda_function.vmbot.arn
}
