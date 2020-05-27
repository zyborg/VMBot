# VMBot
AWS Lambda function to perform various automated tasks in response to EC2 lifecycle events.

## Substitution Evaluation

Many of the values that are used trigger or control operations that drive VMBot operations
can embed _substitution expressions_ that can be resolved in the context of the triggering
operation.  The subtitution expression takes the following form:

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


### EC2 Substitution Evaluation

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

    ```example`30.20.10.1````

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

## Route53 Specification

If a tag with the name `vmbot:r53` is found, its value will be parsed as a
_Record Update Specification_ for the Route53 service.  The specification is
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

If an additional tag with the name `vmbot:r53-routing` is found, it value will be
parsed as a _Routing Policy Specification_ for the associated Route53 record.  The
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
