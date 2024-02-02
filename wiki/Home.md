## Welcome to the CSF.NET wiki!

CSF's main focus is for its users to enjoy writing code with the library and master it with relative ease. 
For this claim to have any value, a great deal is done to help a developer write intuitive and boilerplate-less code, alongside keeping documentation simple and complete.

- Get started with CSF quickly by checking out the **[[Quick Guide|⚡-Quick-Guide]]**.

### 💉 Additional Packages

CSF is not without its own dependencies, but it tries its best to keep all dependencies within a trusted atmosphere, using packages only when they outweigh self-written implementations. So far, it only depends on packages published by official channels of the .NET ecosystem.

#### Dependency Injection

Having grown into a vital part of building effective and modern applications, Dependency Injection (DI) is no less important to be carried along in the equally modern CSF. 
It integrates this feature deeply into its architecture and depends on it to function from the ground up. 

For applications to function with `CSF.Core` or `CSF.Spectre`, it is necessary to install DI functionality through Microsoft's publicized package(s):

```xml
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="" />
```
> This and other required packages can also be installed through the Package Manager, .NET CLI or from within your IDE.

#### Hosting

Carrying out further support within the .NET ecosystem, CSF also introduces a hosting package for deploying apps with the .NET generic host. 

For applications to function with `CSF.Hosting`, it is necessary to install the hosting package that it also implements, also publicized by Microsoft itself:

```xml
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="" />
```

> The hosting extensions package publicized by Microsoft implements the packages necessary for the core component of CSF, and does not expect to have its dependencies implemented alongside it.

*For each of these packages, the minimum version is determined by CSF itself, usually being the latest or equal to the target framework upon which it was released. It is suggested to choose the latest version at time of installing.*

### ⚒️ Customizability

While already fully functional out of the box, this command framework does not shy away from covering extensive applications with more specific needs, which in turn need more than the base features to function according to its developer's expectations. Within CSF, the `CommandManager` can be inherited, opening up a range of protected virtual methods that are applicable during pipeline execution. These can be overridden to modify and customize the pipeline to accompany various needs. 

Alongside this, `CommandContext`'s, `TypeReader`'s, `PreconditionAttribute`'s and `Parser`'s can all be inherited and custom ones created for environmental specifics, custom type interpretation and more.

### ⚡ Performance

While CSF's primary focus is to be the most versatile and easy to use command framework, it also aims to carry high performance throughout the entire execution flow, while keeping source code readable and comprehensible for library contributors and users alike. 
Benchmarks are ran for every version of CSF, with a near complete guarantee that it will not reach higher values upon newly released versions.

> The only cases where optimization will be cast aside is when the developer experience outweighs the need for further optimization. 
> This can be by reason of introducing certain features in the execution pipeline, or by newly introduced steps within said pipeline. In any other cases, be it minor updates or revisions, there will be no change in execution performance.

Benchmarks of the most recent stable release (`CSF 2.0`) show the following results:

|                Method |     Mean |    Error |  StdDev |   Gen0 | Allocated |
|---------------------- |---------:|---------:|--------:|-------:|----------:|
|         Parameterless | 701.7 ns |  8.99 ns | 8.41 ns | 0.1745 |   1.07 KB |
|           Parametered | 884.9 ns | 10.56 ns | 9.36 ns | 0.2060 |   1.27 KB |
| ParameteredUnprovided | 898.5 ns | 10.40 ns | 9.22 ns | 0.2069 |   1.27 KB |
|   NestedParameterless | 693.6 ns |  7.41 ns | 6.94 ns | 0.1745 |   1.07 KB |
|     NestedParametered | 895.2 ns |  5.68 ns | 5.03 ns | 0.2060 |   1.27 KB |

*These benchmarks represent the command execution pipeline from the moment when a parsed `CommandContext` is provided to the framework to be executed, to post execution handling.*

#### Legend:
- Mean      : Arithmetic mean of all measurements.
- Error     : Half of 99.9% confidence interval.
- StdDev    : Standard deviation of all measurements.
- Gen0      : GC Generation 0 collects per 1000 operations.
- Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B).
- 1 ns      : 1 Nanosecond (0.000000001 sec).
