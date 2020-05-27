# VMBot

AWS Lambda function to perform various automated tasks in response to EC2 lifecycle events.

This is useful in automation scenarios such as EC2 state changes in response to Auto-Scale
Group events or any scenario where EC2 state changes may happen independent of any explicit
infrastructure configuration changes.

## Intro

VMBot is a Lambda function that should be deployed to an AWS environment and configured to
respond to EC2 lifecycle events using CloudWatch Events Rules.

VMBot understands the following three
[lifecycle event states](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-instance-lifecycle.html):

* **`running`** - whenever an EC2 instance enters the running state and is ready to operate
* **`stopping`** - whenever an EC2 instance is about to stop or hibernate
* **`shutting-down`** - whwnever an EC2 instance is shutting down in preparation for termination

When VMBot receives an event for one of these EC2 state transitions, it will read the Tags
for that EC2 instance and if it finds any matching _Trigger Tags_, it will perform actions
as prescribed by the corresponding Tag value.

## Trigger Tags and Actions

For this first release, VMBot supports Route 53-related Trigger Tags and Actions.

* **`vmbot:r53`** - used to dynamically create Route 53 records for the associated EC2 instance
* **`vmbot:r53-routing`** - used to supplement the `vmbot:r53` Trigger with Route53 Routing policies
* **`vmbot:r53-health`** - used to create Route 53 Health Checks
