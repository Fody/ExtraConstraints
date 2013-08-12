using ExtraConstraints;

public class ClassWithMethodEnumConstraint 
{
	public void Method<[EnumConstraint] T> ()
	{
        
	}
}

public class ClassWithMethodEnumConstraint2
{
    public void Method<[EnumConstraint(typeof(System.ConsoleColor))] T>()
    {

    }
}