## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/ExtraConstraints/master/Icons/package_icon.png)

Facilitates adding constraints for Enum and Delegate to types and methods.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget 

Nuget package http://nuget.org/packages/ExtraConstraints.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package ExtraConstraints.Fody


### Your Code

    public class Sample
    {
        public void MethodWithDelegateConstraint<[DelegateConstraint] T> ()
        {        
        }
        public void MethodWithEnumConstraint<[EnumConstraint] T>()
        {
        }
    } 
	

### What gets compiled

    public class Sample
    {
        public void MethodWithDelegateConstraint<T>() where T: Delegate
        {
        }

        public void MethodWithEnumConstraint<T>() where T: struct, Enum
        {
        }
    }



## Credit 

Inspired by [Jon Skeets](http://msmvps.com/blogs/jon_skeet)  blog post [Generic constraints for enums and delegates](http://msmvps.com/blogs/jon_skeet/archive/2009/09/10/generic-constraints-for-enums-and-delegates.aspx).

Based manly on code from [Jb Evains](http://evain.net/blog/) post [ Constraining generic constraints](http://evain.net/blog/articles/2012/01/13/constraining-generic-constraints)

## Icon

<a href="http://thenounproject.com/noun/straightjacket/#icon-No7600" target="_blank">Straightjacket</a> designed by <a href="http://thenounproject.com/Luis" target="_blank">Luis Prado</a> from The Noun Project


