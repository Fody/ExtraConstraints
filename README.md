[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/ExtraConstraints.Fody.svg?style=flat)](https://www.nuget.org/packages/ExtraConstraints.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/ExtraConstraints/master/package_icon.png)

Facilitates adding constraints for Enum and Delegate to types and methods.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)


## The nuget package

https://nuget.org/packages/ExtraConstraints.Fody/

    PM> Install-Package ExtraConstraints.Fody


### Your Code

    public class Sample
    {
        public void MethodWithDelegateConstraint<[DelegateConstraint] T> () {...}

        public void MethodWithTypeDelegateConstraint<[DelegateConstraint(typeof(Func<int>))] T> () {...}

        public void MethodWithEnumConstraint<[EnumConstraint] T>() {...}

        public void MethodWithTypeEnumConstraint<[EnumConstraint(typeof(ConsoleColor))] T>() {...}
    } 
	

### What gets compiled

    public class Sample
    {
        public void MethodWithDelegateConstraint<T>() where T: Delegate {...}

        public void MethodWithTypeDelegateConstraint<T>() where T: Func<int> {...}

        public void MethodWithEnumConstraint<T>() where T: struct, Enum {...}

        public void MethodWithTypeEnumConstraint<T>() where T: struct, ConsoleColor {...}
    }


## Credit 

Inspired by [Jon Skeets](http://msmvps.com/blogs/jon_skeet)  blog post [Generic constraints for enums and delegates](http://msmvps.com/blogs/jon_skeet/archive/2009/09/10/generic-constraints-for-enums-and-delegates.aspx).

Based mainly on code from [Jb Evain](http://evain.net/bio/)'s blog post [Constraining generic constraints](http://evain.net/blog/articles/2012/01/13/constraining-generic-constraints).


## Icon

<a href="http://thenounproject.com/noun/straightjacket/#icon-No7600" target="_blank">Straightjacket</a> designed by <a href="http://thenounproject.com/Luis" target="_blank">Luis Prado</a> from The Noun Project.
