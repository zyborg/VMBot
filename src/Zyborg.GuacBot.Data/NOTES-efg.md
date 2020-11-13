# NOTES - EntityFrameworkGenerator (EFG)

## Initializing Config YAML

```PWSH
PS> efg initialize -p postgresql -c 'SomeConnectionString' --name 'GuacDB' -d . -f guacdb-pg.efg.yaml
PS> efg initialize -p mysql      -c 'server=localhost;user=guac_user;password=password;database=guac1' --name 'GuacDB' -d . -f guacdb-my.efg.yaml
```
