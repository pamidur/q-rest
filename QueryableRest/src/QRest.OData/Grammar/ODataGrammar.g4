grammar ODataGrammar;

parse
 : queryOptions EOF
 ;


queryOptions : queryOption ( AMPERSAND queryOption )*; 

queryOption :
  filter 
  | count
;

filter : DOLLAR 'filter' EQPARAM filterexpr=expression ;

count : DOLLAR 'count' EQPARAM decexpr=bool ;

expression
 : LPAREN expression RPAREN                       #parenExpression
 | NOT expression                                 #notExpression
 | left=expression op=comparator right=expression #comparatorExpression
 | left=expression op=binary right=expression     #binaryExpression
 | bool                                           #boolExpression
 | IDENTIFIER                                     #identifierExpression
 | DECIMAL                                        #decimalExpression
 | INT                                            #intExpression
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
AND        : 'AND' | 'and' ;
OR         : 'OR' | 'or' ;
NOT        : 'NOT' | '!' | '<>' | 'not' ;
TRUE       : 'TRUE' | 'true' ;
FALSE      : 'FALSE' | 'false' ;
GT         : 'gt' | 'GT' ;
GE         : 'ge' | 'GE' ;
LT         : 'lt' | 'LT' ;
LE         : 'le' | 'LE' ;
EQ         : 'eq' | 'EQ' ;
NE         : 'ne' | 'NE' ;
LPAREN     : '(' ;
RPAREN     : ')' ;
DECIMAL    : INT '.' [0-9]+  ;
INT        : '-'? [0-9]+ ;
IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]* ;
STRINGLITERAL : SQ [a-zA-Z_0-9]* SQ ;
WS         : [ \r\t\u000C\n]+ -> skip;
