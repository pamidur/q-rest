# queryable-rest

```
:filter(text:eq(qwerty),text:contains(@a)):select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(*,internal:select)):sort(blocks:max(id)),a:let(1)
```

Ideas:
- oData sucks
- we need to make the whole url to be without escape symbols (less manual mistakes, QA will be happy)
- put hole query into single http query param (so we separate webapi param parsing from actual query parsing)
- the query is a chain of method calls
- one can call method by using ```:method``` or with params ```:method(param)``` or with multiple params ```:method(param1, param2)```
- ```:select``` == ```:select(*)``` == odata's ```$expand```

