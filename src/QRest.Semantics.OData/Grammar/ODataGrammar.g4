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
 | bool                                           #boolExpression
 | IDENTIFIER                                     #identifierExpression
 | DECIMAL                                        #decimalExpression
 | INT                                            #intExpression
 | dateTimeOffset								  #dateTimeOffsetExpression
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

dateTimeOffset : 
	year MINUS month MINUS day 'T' hour COLON minute ( COLON second ( '.' fractionalSeconds )? )? ( 'Z' | SIGN hour COLON minute );

year : (Digit) (Digit) (Digit) (Digit) ;

month : '0' ONE_TO_NINE | '1' ZERO_TO_TWO;

day   : ZERO_TO_TWO ONE_TO_NINE
      | '3' ZERO_TO_ONE;

hour   : ZERO_TO_ONE ( Digit )
       | '2' ONE_TO_THREE; 

minute : ZERO_TO_FIFTY_NINE;

second : ZERO_TO_FIFTY_NINE;       

fractionalSeconds : ( Digit )+;

functionParams
 :   expression (COMMA expression)*
 ;

Digit		: [0-9];
Alpha		: [a-zA-Z];

ONE_TO_TWO  : [1-2];
ONE_TO_THREE: ONE_TO_TWO | [3];
ONE_TO_FOUR : ONE_TO_THREE | [4]; 
ONE_TO_NINE : ONE_TO_FOUR | [5-9]; 
ZERO_TO_ONE : [0-1];
ZERO_TO_TWO : ZERO_TO_ONE | [2];
ZERO_TO_THREE: ZERO_TO_TWO | [3];
ZERO_TO_FOUR: ZERO_TO_THREE | [4];
ZERO_TO_FIVE: ZERO_TO_FOUR | [5];
ZERO_TO_FIFTY_NINE: ZERO_TO_FIVE Digit;

COLON	   : ':' ;
SIGN	   : (PLUS | MINUS);
MINUS	   : '-' ;
PLUS	   : '+' ;
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
STRINGLITERAL : SQ [a-zA-Z_0-9\-:.+]* SQ ;
WS         : [ \r\t\u000C\n]+ -> skip;
