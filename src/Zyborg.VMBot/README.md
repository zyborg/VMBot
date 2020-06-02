# README - VMBot Usage

AWS Lambda function to perform various automated tasks in response to EC2 lifecycle events.

VMBot works by parsing well-known _Trigger Tags_ to perform specific Actions.  This section
describes the Trigger Tags and how they are parsed, as well as the Actions they drive.

Jump to a section:

* [Substitution Evaluation](#substitution-evaluation)
  * [Common Substitutions](#common-substitutions)
  * [EC2 Substitutions](#ec2-substitutions)
* [Route53 Specifications](#route53-specifications)
  * [Record Update Specification](#record-update-specification)
  * [Routing Policy Specification](#routing-policy-specification)
  * [Health Check Specification](#health-check-specification)
* [EIP Specification](#eip-specification)

----

## Substitution Evaluation

Many of the values that are used trigger or control operations that drive VMBot operations
can embed _substitution expressions_ that can be resolved in the context of the triggering
operation.  The substitution expression takes the following form:

```code
'%' <substitution-key> [ ':' <substitution-arg> ] '%'
```

The _substitution key_ is required and identifies the value that will be substituted for
the expression.  For an unrecognized key, the entirety of the expression including the
delimiter tokens (`%`) will be left in place.  Some keys may require or optionally support
a _substitution argument_ to further refine the substitution value.  See the documentation
for each key to determine if and how a substitution argument is supported.

### Common Substitutions

For all contexts, the following substitution keys are supported:

#### Date & Time Related

* **`NOW_DATE`** - Resolves to the current date as a result
    of `DateTime.Now` using the format `yyyy_MM_dd`.
    The substitution argument is ignored.

    ```example
        `2020_05_22` representing the date May 22, 2020
    ```

* **`NOW_TIME`** - Resolves to the current time as a result
    of `DateTime.Now` using the format `HH_mm_ss`.
    The substitution argument is ignored.

    ```example
        `14_24_33` representing the time 2:24:33pm
    ```

* **`UTC_DATE`** - Resolves to the current UTC date as a result
    of `DateTime.UtcNow` using the format `yyyy_MM_dd`.
    The substitution argument is ignored.

    ```example
        `2020_05_22` representing the date May 22, 2020
    ```

* **`UTC_TIME`** - Resolves to the current UTC time as a result
    of `DateTime.UtcNow` using the format `HH_mm_ss`.
    The substitution argument is ignored.

    ```example
        `14_24_33` representing the time 2:24:33pm
    ```

* **`NOW`** - Resolves to the current datetime as a result
    of `DateTime.Now` optionally using the substitution
    argument as the format.  If no substitution argument is
    provided, the default format is `yyyyMMdd_HHmmss`.

    ```example
        `20200522_142433` representing the date
        May 22, 2020 at 2:24:33pm.
    ```

* **`UTC`** - Resolves to the current datetime as a result
    of `DateTime.UtcNow` optionally using the substitution
    argument as the format.  If no substitution argument is
    provided, the default format is `yyyyMMdd_HHmmss`.

    ```example
        `20200522_142433` representing the date
        May 22, 2020 at 2:24:33pm.
    ```

#### Environment & Target Properties

* **`ENV`** - Resolves to the value of an environment variable
    specified by the substitution argument.
    If the environment variable is missing or otherwise resolves
    to null, the expression resolves to the original substitution
    expression.

    ```example
        using an expression value of
        `%ENV:EnvironmentName%` would evaluate to the
        value `Debug` when executing under the Debug
        configuration.
    ```

* **`ENV?`** - This key resolves exactly as the `ENV`
    key except that a resolved null-value resolution
    will resolve to the empty string.
* **`PROP`** - Resolves to the value of the property whose
    name is specified by the substitution argument applied
    If the named property does not exist on the target type
    or the property value resolve to `null`, the
    expression resolves to the original substitution
    expression.

    ```example
        using an expression value of
        `%PROP:ProcessName%` applied to an instance of
        the `System.Diagnostics.Process` class would
        resolve to the associated process' name.
    ```

* **`PROP?`** - This key resolves exactly as the `PROP`
    key except that a resolved null-value resolution
    will resolve to the empty string.

#### Escape Forms

* **_(empty)_** - The _empty_ key (written as two adjacent
    tokens with no other intermediate characters, resolves
    to a single percent (`%`) character.

* **`PCT`** - Resolves to one or more percent (`%`) characters.
    This key optionally supports the substitution argument, and
    if provided, is interpreted as the number of percent characters
    to resolve to.  If not provided, it defaults to 1.

* **`ESC`** - This key is used to render a string of one or
    more literal or escaped characters.  Escaped characters
    a represented in this form:

    ```example
        '#' <hex-code>
    ```

    Where the `hex-code` is the ASCII code for the character
    as a zero-padded, tw0-digit hexadecimal value.  For example

    ```example
        `#44#6F#67` would resolve to the 3-character string `Dog`
        `Cats #26 Dogs` would resolve to the string `Cats & Dogs`
        `#3b #23 #25 #26 #3a` resolves to the string `; # % & :`
    ```

### EC2 Substitutions

When applied in the context of an EC2 instance, in addition to the
[Common Substitution](#common-substitutions) keys, the following
keys are also supported:

* **`ID`** - Resolves to the Instance ID.
    An optional RegEx expression can be specified as
    the Substitution Argument to transform the ID.  If
    specified, the Argument should be specified as two
    components a match expression and a replacement expression
    separated by a semicolon.  The following examples depict
    the result when evaluated against an ID of `i-123456abcdefg`.

    ```example
        An expression of `%ID%` gives you `i-123456abcdefg`.
    ```

    ```example
        An expression of `%ID:(....)$;\1%` gives you `defg`.
    ```

    ```example
        An expression of `%ID:(^.{6}).*(.{4}$);\1...\2%` gives you `i-1234...defg`.
    ```

* **`PRIVATE_IP`** - Resolves to the private IP address of the Target Instance.
    The substitution argument is ignored.

    ```example
        `10.10.10.10`
    ```

* **`PUBLIC_IP`** - Resolves to the public IP address of the Target Instance.
    The substitution argument is ignored.

    ```example
        `30.20.10.1`
    ```

* **`PRIVATE_DNS`** - Resolves to the private DNS name of the Target Instance.
    The substitution argument is ignored.

    ```example
        `ip-10-10-10-10.ec2.internal`
    ```

* **`PUBLIC_DNS`** - Resolves to the public DNS name of the Target Instance.
    The substitution argument is ignored.

    ```example
        `ec2-30-20-10-1.compute-1.amazonaws.com`
    ```

* **`LAUNCH_TIME`** - Resolves to the Launch Time of the Target Instance.
    The substitution argument can optionally specify a `DateTime`
    format string.
        using an expression value of
        `%LAUNCH_TIME:yyyyMMdd_HHmmss%` could evaluate to the
        value `20200522_161430`

* **`TAG`** - Evaluates to the value of a resource tag with the name
    specified by the substitution argument.
    If the tag is missing or otherwise resolves to null,
    the expression resolves to the original substitution
    expression.

    ```example
        using an expression value of
        `%TAG:Name%` could evaluate to the
        value `My_First_VM`
    ```

* **`TAG?`** - This key resolves exactly as the `TAG`
    key except that a resolved null-value resolution
    will resolve to the empty string.

## Route53 Specifications

Several tags are available to manage various resources in the are
of the Route 53 service.

### Record Update Specification

If a tag with the name `vmbot:r53` is found, its value will be parsed as a
_Record Update specification_ for the Route53 service.  The specification is
a string with the following components:

```code
<zone-id> ';' <record-name> [ ';' [ <record-type> ] [ ';' [ <record-ttl> ] [ ';' <record-value> ] ] ]
```

Briefly, the specification contains multiple components, each separated by a
semicolon (`;`).  The first two components (`zone-id` and `record-name`) are
required, but the remaining components are optional.  For the optional
components, you can _skip_ that component by providing the preceding semicolon
for that component but leaving the component value empty.  This allows you, for
example, to specify a `record-value` without requiring the intermediate optional
components of `record-type` and `record-ttl`.

Here are the details of each component:

* **`zone-id`** - (Required) ID of the zone where the record will be edited; it is
  critical that the VMBot Lambda function has the necessary permissions via
  its assigned IAM Role to access the referenced Zone.  By default only the
  Zones in the same AWS Account are made accessible to VMBot.

* **`record-name`** - (Required) Full name of the DNS record that will be edited.

* **`record-type`** - (Optional) Type of the DNS record to be edited.  If unspecified
  defaults to an `A` record.  You can specify any supported type (such as `CNAME`,
  or `TXT`).

* **`record-ttl`** - (Optional) The Time-To-Live value to apply to the DNS record
  specified in seconds. If unspecified defaults to 60.

* **`record-value`** - (Optional) The value to apply to the DNS record.
  If unspecified, defaults to the Public IP address of the EC2 instance if it
  has one, or the Private IP address of the EC2 instance otherwise.

Before parsing the specification in the Tag value, the Tag value will be evaluated
for EC2 _substitutions_ relative to the target EC2 instance.

### Routing Policy Specification

If an additional tag with the name `vmbot:r53-routing` is found, its value will be
parsed as a _Routing Policy specification_ for the associated Route53 record.  The
specification is a string with the following components:

```code
    <set-id> ';' <route-type> [ ';' <route-type-arg> ]
```

The `route-type` can be one of the following values, and will determine
if the `route-type-arg` is required and how it is interpreted:

* **`MULTI`** - `route-type-arg` is optional and ignored.
            You can have up to 8 records configured to use
            the multi-valued record routing policy

* **`WEIGHTED`** - `route-type-arg` is required and interpreted as
            a relative numerical weight in the range 0-255.

* **`FAILOVER`** - `route-type-arg` is required and if its value is
            the string `PRIMARY` (compared case-insensitively) then it is
            configured as a primary failover record, otherwise it will be
            configured as a secondary record.

* **`LATENCY`** - `route-type-arg` is required and should specify
            the AWS Region from which the latency check will be performed.

* **`GEO`** - `route-type-arg` is required and should be specified
            in one of two forms:

    ```code
        'continent=' <content-code>
        ## e.g.
        ##    `content=NA` for North America
        ##    `content=AS` for Asia

        'country=' <country-code> [ ',' <sub-location> ]
        ## e.g.
        ##    `country=US,MD` for Maryland in the United States
        ##    `country=FR` for France
    ```

> NOTE: Some of the routing policies supported above may also
> require an associated Health Check Specification (as described below)
> in order to work properly.

### Health Check Specification

If a tag with the name `vmbot:r53-health` is found, its value will be parsed as a
_Health Check specification_ for the associated EC2 instance.  The specification
can take one of two forms.

> NOTE: a health check can be specified and created for an EC2 instance even if
> there is no associated Route 53 Record Update or Routing Policy tags provided
> to make use of it.  However, several of the Routing Policy types do require a
> Health Check in order to work properly.

#### Inline (Short) Form

The _Inline_ form allows you to define a Health Check directly in the Tag value
however it is limited to only a subset of Health Check types and limited to only
a subset attributes.  The Inline form has the following components:

```code
    <hc-type> ';' <threshold> ';' <ip-addr> ';' <port> ';' <fqdn> ';' <res-path> ';' <search>
```

> For details on the following options, please reference the detailed [AWS documentation
  ](https://docs.aws.amazon.com/Route53/latest/APIReference/API_HealthCheckConfig.html).

The **`hc-type`** indicates the Health Check type and is limited to the following values
in the Inline form:

* `HTTP`
* `HTTP_STR_MATCH`
* `HTTPS`
* `HTTPS_STR_MATCH`
* `TCP`
  
Depending on the Type selected, the subsequent additional components may or may
not be supported or required (see the official docs for more details).  For each
component, you can skip that value by simply omitting it, but you must provide
the successive semicolon (`;`) in order to specify any other components that follow.
The remaining components supported in the Inline form are:

| Component | Corresponding Configuration Property
|-|-|
| **`threshold`** | `FailureThreshold`
| **`ip-addr`**   | `IPAddress`
| **`port`**      | `Port`
| **`fqdn`**      | `FullyQualifiedDomainName`
| **`res-path`**  | `ResourcePath`
| **`search`**    | `SearchString`

#### S3 Reference (Full) Form

Instead of specifying the Health Check specification directly inline in the Tag value
you can also use an alternate S3 Reference value that points to an S3 file object **that
the VMBot should have read access to**.  The S3 file object will be interpreted as a
JSON file that will be deserialized to a complete [HealthCheckConfig
](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Route53/THealthCheckConfig.html)
instance.

The S3 Reference form is specified as follows:

```code
    '!S3=' <bucket-name> '/' <object-key>
```

An example of this form:

```example
    !S3=example-bucket/the/path/to/hc-config.json
```

In this example the bucket name is `example-bucket` and the full object key is
`the/path/to/hc-config.json`.

Just prior to parsing the resolved S3 object as JSON content, the complete S3
object file will be first evaluated for _Substitutions_ in a similar manner to
the way that Tag values are first resolved.  The Substitutions will be resolved
in the context of the target EC2 instance.  This allows you to embed
context-specific and EC2-specific values within.  Because of this, make sure that
you escape any of the special Substitution tokens properly.

## EIP Specification

> TODO:  this feature is in progress
