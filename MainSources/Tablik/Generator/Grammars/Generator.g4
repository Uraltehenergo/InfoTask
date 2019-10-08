grammar Generator;

/* Parser Rules */

expr: element* EOF                #CombinedExpr;

element : tag                    #ElementTag		 
 			| combtext           #ElementText;

combtext : text+              #CombinedText;

text: (PARENT | CHILDREN | CHILDRENCOND | OR | AND | NOT | OPER | LPAREN | RPAREN | DOT) #GetText
        | (IDENT | NUMBER | TEXT | SPACE)																						     	#GetText
		| (STRING | SIGNAL | COMMENT)                                                                                                 #GetText
		| SPECIAL																																        #GetSpecial;

tag : BTAG PARENT WS* BTAG expr ETAG WS* ETAG                            #TagParent
     | BTAG SPACE PARENT WS* BTAG expr ETAG WS* ETAG                 #TagParent
	 | BTAG CHILDREN WS* BTAG expr ETAG WS* BTAG expr ETAG WS* ETAG                        #TagChildrenSep
	 | BTAG SPACE CHILDREN WS* BTAG expr ETAG WS* BTAG expr ETAG WS* ETAG             #TagChildrenSep
	 | BTAG COND WS* BTAG WS* cond WS* ETAG WS* BTAG expr ETAG WS* BTAG expr ETAG WS* ETAG               #TagCondSep
	 | BTAG SPACE COND WS* BTAG WS* cond WS* ETAG WS* BTAG expr ETAG WS* BTAG expr ETAG WS* ETAG    #TagCondSep
	 | field                                                       #TagField;

cond: mean OPER WS* mean                       #CondMean
       | mean SPACE OPER WS* mean           #CondMean
	   | LPAREN WS* cond WS* RPAREN         #CondParen           	      	
	   | NOT WS* cond                                    #CondNot	
	   | cond WS* OR WS* cond                       #CondOr	   
	   | cond WS* AND WS* cond                    #CondAnd;	   

field : BTAG IDENT WS* ETAG                        #FieldSimple
        | BTAG SPACE IDENT WS* ETAG             #FieldSimple
		| BTAG IDENT DOT TOIDENT WS* ETAG      #FieldToIdent
		| BTAG SPACE IDENT DOT TOIDENT WS* ETAG     #FieldToIdent;

mean: field                                  #MeanField
       | NUMBER                           #MeanNumber                    
       | STRING                           #MeanString;
	   	
/* Lexer Rules */

fragment LETTER : [_a-zA-Z�-��-�];
fragment DIGIT : [0-9];
fragment WS :  (' ' | '\t' | '\r' | '\n');
fragment SYMB: ( LETTER | DIGIT | '+' | ';' | ':' | ',' | '-' | '^' ) ;

SPECIAL :'\\{' | '\\}' | '\\\'' |'\\/*' | '\\*/'; 
STRING : '\'' .*? '\'';
SIGNAL: '{' .*? '}';
COMMENT: '/*' .*? '*/';
BTAG: '[';
ETAG: ']';
SPACE: WS+;

PARENT : [Uu][Pp][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];
CHILDREN : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];
COND : [Ss][Uu][Bb][Tt][Aa][Bb][Ll][Cc][Oo][Nn][Dd] | [��][��][��][��][��][��][��][��][��][��];

LPAREN : '(';
RPAREN : ')';
DOT : '.';
OPER: '==' | '<>' | '<' | '>' | '<=' | '>=';
OR : [Oo][Rr] | [��][��][��];
AND: [Aa][Nn][Dd] | [��];
NOT: [Nn][Oo][Tt] | [��][��];
TOIDENT :[Tt][Oo][Ii][Dd][Ee][Nn][Tt];

NUMBER: DIGIT+;
IDENT: (LETTER | DIGIT)+;	 
TEXT: '*' | '/' | '=' | (SYMB+); 	
