[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat&max-age=86400)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/ExtraConstraints.Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExtraConstraints.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Home/)

![Icon](https://raw.githubusercontent.com/Fody/ExtraConstraints/master/package_icon.png)

Facilitates adding constraints for Enum and Delegate to types and methods.


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
<?xml version="1.0" encoding="utf-8" ?>
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

Inspired by [Jon Skeets](http://msmvps.com/blogs/jon_skeet)  blog post [Generic constraints for enums and delegates](http://msmvps.com/blogs/jon_skeet/archive/2009/09/10/generic-constraints-for-enums-and-delegates.aspx).

Based mainly on code from [Jb Evain](http://evain.net/bio/)'s blog post [Constraining generic constraints](http://evain.net/blog/articles/2012/01/13/constraining-generic-constraints).


## Icon

<a href="http://thenounproject.com/noun/straightjacket/#icon-No7600" target="_blank">Straightjacket</a> designed by <a href="http://thenounproject.com/Luis" target="_blank">Luis Prado</a> from The Noun Project.
