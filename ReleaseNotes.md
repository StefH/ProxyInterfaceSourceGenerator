# 0.0.22 (09 May 2022)
- [#37](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/37) - Add support for indexers [enhancement] contributed by [StefH](https://github.com/StefH)
- [#13](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/13) - no support for indexers [bug]

# 0.0.21 (08 May 2022)
- [#36](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/36) - If the parameter is defined as Nullable, always return &quot;null&quot; as default value. [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.20 (08 May 2022)
- [#35](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/35) - The new() constraint must be the last constraint specified [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.19 (08 May 2022)
- [#34](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/34) - Fix default valeu for reference types and non-reference types [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.18 (08 May 2022)
- [#33](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/33) - Fix for default parameter (default) [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.17 (07 May 2022)
- [#32](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/32) - Add support for 'file-scoped' namespaces [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.16 (06 May 2022)
- [#31](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/31) - Fix for Explicit DefaultValue is not defined [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.15 (06 February 2022)
- [#30](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/30) - Fixed TryFindProxyDataByTypeName [bug] contributed by [StefH](https://github.com/StefH)

# 0.0.14 (04 February 2022)
- [#29](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/29) - Add support for base (proxy) class [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.13 (02 February 2022)
- [#28](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/28) - Add support for static properties and methods [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.12 (01 February 2022)
- [#27](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/27) - ProxyBaseClasses [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.11 (10 August 2021)
- [#26](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/26) - Update filename for generated interface files + set DevelopmentDependency to true for the project [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.10 (06 August 2021)
- [#25](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/25) - Fix support for Nullable (language version 8) [bug] contributed by [StefH](https://github.com/StefH)
- [#14](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/14) - for projects where #nullable is disabled emitting nullable reftype without preprocessor '#nullable enable' would result in compile time error. [bug]

# 0.0.9 (05 August 2021)
- [#24](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/24) - Add &lt;auto-generated&gt; [enhancement] contributed by [StefH](https://github.com/StefH)

# 0.0.8 (03 August 2021)
- [#23](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/23) - Add support for Events [enhancement] contributed by [StefH](https://github.com/StefH)
- [#8](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/8) - no support for events... maybe simple forwarders [enhancement]

# 0.0.7 (02 August 2021)
- [#22](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/22) - Add support for using a simple type-name [enhancement] contributed by [StefH](https://github.com/StefH)
- [#3](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/3) - it is not allowed to put simple type name but only full name [bug]

# 0.0.6 (01 August 2021)
- [#20](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/20) - Add support for generics [enhancement] contributed by [StefH](https://github.com/StefH)
- [#6](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/6) - no support for generics [bug]

# 0.0.5 (31 July 2021)
- [#18](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/18) - Add support for reserved keywords like @object and @string [enhancement] contributed by [StefH](https://github.com/StefH)
- [#19](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/19) - Fix Default value [enhancement] contributed by [StefH](https://github.com/StefH)
- [#9](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/9) - Support verbatim / reserved names like @object [bug]
- [#11](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/11) - interface methods do not propagate default value for parameters [bug]

# 0.0.4 (28 July 2021)
- [#15](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/15) - Fix issue #4 (string?) [bug] contributed by [StefH](https://github.com/StefH)
- [#16](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/16) - Fix 'params' [bug] contributed by [StefH](https://github.com/StefH)
- [#17](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/17) - Fix method parameters: 'in', 'out' and 'ref' [bug] contributed by [StefH](https://github.com/StefH)
- [#4](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/4) - mapper is used for `string?` / there is a case where _mapper is used but not assigned [bug]
- [#10](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/10) - &quot;ref&quot; and &quot;out&quot; are not being forwarded correctly [bug]
- [#12](https://github.com/StefH/ProxyInterfaceSourceGenerator/issues/12) - &quot;params&quot; keyword is not emitted [bug]

# 0.0.3 (25 July 2021)
- [#1](https://github.com/StefH/ProxyInterfaceSourceGenerator/pull/1) - Fix namespace [bug] contributed by [StefH](https://github.com/StefH)

