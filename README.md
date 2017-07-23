# Urunium.Redux
Redux implementation in c#
## Alternatives
Other more matured alternatives are available:
- https://github.com/GuillaumeSalles/redux.NET
- https://github.com/pshomov/reducto
# Goal
- Redux implementation in c#
- More object-oriented and idiomatic C#

# Documentation
Please visit the wiki https://github.com/urunium/Urunium.Redux/wiki for documentations.

High level code example:

```c#
public class Reducer : IReducer<int>
{
	public int Apply(int previousState, object action)
	{
		switch(action)
		{
			case string action when action == "+":
				return previousState + 1;
			case string action when action == "-":
				return previousState - 1;
		}
	}
}
// Now assign reducer to store and dispatch actions:
Store<int> store = new Store<int>(new Reducer(), 0);
store.Dispatch("+");
Console.WriteLine(store.State); // Should be 1
store.Dispatch("+");
Console.WriteLine(store.State); // Should be 2
store.Dispatch("-");
Console.WriteLine(store.State); // Should be 1
store.Dispatch("-");
Console.WriteLine(store.State); // Should be 0
```
