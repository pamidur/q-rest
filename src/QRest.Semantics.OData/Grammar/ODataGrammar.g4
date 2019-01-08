grammar ODataGrammar;

parse
 : queryOptions EOF
 ;


queryOptions : queryOption ( AMPERSAND queryOption )*; 

queryOption :
  filter 
  | select
  | count
  | orderby
  | top
  | skip
;

filter : DOLLAR 'filter' EQPARAM filterexpr=expression ;

select: DOLLAR 'select' EQPARAM selectItem ( COMMA selectItem )* ;

selectItem : IDENTIFIER ;

count : DOLLAR 'count' EQPARAM decexpr=bool ;

orderby : DOLLAR 'orderby' EQPARAM orderbyItem ( COMMA orderbyItem) ;

orderbyItem : IDENTIFIER order? ;

order : 'asc' | 'desc' ;

top : DOLLAR 'top' EQPARAM INT ;

skip : DOLLAR 'skip' EQPARAM INT ;

expression
 : LPAREN expression RPAREN                       #parenExpression
 | NOT expression                                 #notExpression
 | left=expression op=comparator right=expression #comparatorExpression
 | left=expression op=binary right=expression     #binaryExpression
 | DATETIME										  #datetimeExpression
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

DATETIME   : DIGIT4 '-' DIGIT2 '-' DIGIT2 'T' DIGIT2 ':' DIGIT2 ':' DIGIT2 ('.' [0-9]+)? ( 'Z' | ('+'|'-') DIGIT2 ':' DIGIT2 ) ;
EQPARAM	   : '=' ;
DOLLAR     : '$' ;
AMPERSAND  : '&' ;
COMMA      : ',' ;
SQ         : '\'' ;
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
LQPAREN	   : '[' ;
RQPAREN	   : ']' ;
DECIMAL    : INT '.' [0-9]+  ;
INT        : '-'? [0-9]+ ;
DIGIT	   : [0-9] ;
DIGIT2     : DIGIT DIGIT;
DIGIT4     : DIGIT2 DIGIT2 ;
IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]* ;
STRINGLITERAL : SQ .*? SQ ;
WS         : [ \r\t\u000C\n]+ -> skip;
