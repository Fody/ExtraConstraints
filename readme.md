# <img src="/package_icon.png" height="30px"> ExtraConstraints.Fody

[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/ExtraConstraints.Fody.svg)](https://www.nuget.org/packages/ExtraConstraints.Fody/)

Facilitates adding constraints for Enum and Delegate to types and methods.


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [ExtraConstraints.Fody NuGet package](https://nuget.org/packages/ExtraConstraints.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package ExtraConstraints.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<ExtraConstraints/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
  <ExtraConstraints/>
</Weavers>
```


### Your Code

```csharp
public class Sample
{
    public void MethodWithDelegateConstraint<[DelegateConstraint] T> () {...}

    public void MethodWithTypeDelegateConstraint<[DelegateConstraint(typeof(Func<int>))] T> () {...}

    public void MethodWithEnumConstraint<[EnumConstraint] T>() {...}

    public void MethodWithTypeEnumConstraint<[EnumConstraint(typeof(ConsoleColor))] T>() {...}
}
```


### What gets compiled

```csharp
public class Sample
{
    public void MethodWithDelegateConstraint<T>() where T: Delegate {...}

    public void MethodWithTypeDelegateConstraint<T>() where T: Func<int> {...}

    public void MethodWithEnumConstraint<T>() where T: struct, Enum {...}

    public void MethodWithTypeEnumConstraint<T>() where T: struct, ConsoleColor {...}
}
```


## Credit

Inspired by [Jon Skeets](http://msmvps.com/blogs/jon_skeet) blog post [Generic constraints for enums and delegates](http://msmvps.com/blogs/jon_skeet/archive/2009/09/10/generic-constraints-for-enums-and-delegates.aspx).

Based mainly on code from [Jb Evain](http://evain.net/bio/)'s blog post [Constraining generic constraints](http://evain.net/blog/articles/2012/01/13/constraining-generic-constraints).


## Icon

[Straightjacket](https://thenounproject.com/noun/straightjacket/#icon-No7600) designed by [Luis Prado](https://thenounproject.com/Luis) from [The Noun Project](https://thenounproject.com).