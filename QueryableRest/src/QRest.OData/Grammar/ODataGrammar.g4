grammar ODataGrammar;

parse
 : queryOptions EOF
 ;


queryOptions : queryOption ( AMPERSAND queryOption )*; 

queryOption  : odataQueryOption  ;

odataQueryOption :
  filter 
  | count
;

filter : DOLLAR 'filter' EQPARAM filterexpr=expression ;

count : DOLLAR 'count' EQPARAM bool ;

expression
 : LPAREN expression RPAREN                       #parenExpression
 | NOT expression                                 #notExpression
 | left=expression op=comparator right=expression #comparatorExpression
 | left=expression op=binary right=expression     #binaryExpression
 | bool                                           #boolExpression
 | IDENTIFIER                                     #identifierExpression
 | DECIMAL                                        #decimalExpression
 | STRINGLITERAL                                  #stringExpression
 | func=IDENTIFIER LPAREN functionParams RPAREN   #funcCallExpression
 ;

comparator
 : GT | GE | LT | LE | EQ
 ;

binary
 : AND | OR
 ;

bool
 : TRUE | FALSE
 ;

functionParams
 :   expression (COMMA expression)*
 ;

EQPARAM	   : '=' ;
DOLLAR     : '$' ;
AMPERSAND  : '&' ;
COMMA      : ',' ;
SQ         : ['];
AND        : 'AND' ;
OR         : 'OR' ;
NOT        : 'NOT' | '!' | '<>' ;
TRUE       : 'TRUE' ;
FALSE      : 'FALSE' ;
GT         : 'gt' ;
GE         : 'ge' ;
LT         : 'lt' ;
LE         : 'le' ;
EQ         : 'eq' ;
LPAREN     : '(' ;
RPAREN     : ')' ;
DECIMAL    : '-'? [0-9]+ ( '.' [0-9]+ )? ;
IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]* ;
STRINGLITERAL : SQ [a-zA-Z_0-9]* SQ ;
WS         : [ \r\t\u000C\n]+ -> skip;
