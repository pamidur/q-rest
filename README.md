# queryable-rest 

Examples
```
http://mydata.com/api/cars/:filter(color:eq('red')):select(id,price)

http://mydata.com/api/cars?query=:filter(color:eq('red')):select(id,price)
```

Both cases don't require escaping symbols

Bigger one 
```
:filter(text:eq(qwerty),text:contains(@a)):select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(*,internal:select)):sort(blocks:max(id)):skip(3):take(10),a:let(1)
```
Let me split it for you for better readability
```
:filter(text:eq('qwerty'),text:contains(@a))
:select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(*,internal:select))
:sort(blocks:max(id))
:skip(3)
:take(10)
,a:let(1)
```

Design notes:
- System is extensible by adding new operations
- The operation is a "static method" with context as a first argument (like c# extension methods)

Ideas:
- oData sucks
- we need to make the whole url to be without escape symbols (less manual mistakes, QA will be happy)
- put hole query into single http query param (so we separate webapi param parsing from actual query parsing)
- the query is a chain of method calls, metheds are chained by colons
- one can call method by using ```:method``` or with params ```:method(param)``` or with multiple params ```:method(param1, param2)```
- ```:select``` == ```:select(*)``` == odata's ```$expand```
- all complex properties are expandable (collapsed by default) despite the database behind
- method call are translated into linq Expressions. System is extensible with custom expressions.
- chain may ends aggregation method such as ```:count``` and ```:max```

- system may have more then one semantics and parser, but after url processing we should and up with object representation of method chain
