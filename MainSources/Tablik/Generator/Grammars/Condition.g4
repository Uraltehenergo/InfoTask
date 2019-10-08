grammar Condition;

/* Parser Rules */

text : expr EOF                           #TextExpr
       | expr                                   #TextPartial;

subtext : children EOF                 #SubTextExpr
            | children                        #SubTextPartial;

expr : IDENT                                           #ExprIdent
        | IDENT BTAG PARENT ETAG         #ExprParent 
        | IDENT props                                 #ExprProps;  		
        
props : BTAG cond ETAG                  #PropsCond
		 | children                                #PropsChildren
		 | BTAG cond ETAG children     #PropsCondChildren;

children: BTAG CHILDREN ETAG             #GetChildren
			| BTAG CHILDREN ETAG props 	#GetChildrenProps;	

cond: mean OPER mean                 #CondMean                  
	   | LPAREN cond RPAREN       #CondParen           	      	
	   | NOT cond                          #CondNot	
	   | cond OR cond                     #CondOr	   
	   | cond AND cond                   #CondAnd;	 
	   
mean: BTAG IDENT WS* ETAG               #MeanField
       | BTAG SPACE IDENT WS* ETAG   #MeanField
       | NUMBER                            #MeanNumber                    
       | STRING                             #MeanString;

/* Lexer Rules */

fragment LETTER : [_a-zA-Z�-��-�];
fragment DIGIT : [0-9];

BTAG: '[';
ETAG: ']';

WS :  (' ' | '\t' | '\r' | '\n') -> skip;

PARENT : [Uu][Pp][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];
CHILDREN : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [��][��][��][��][��][��][��];

LPAREN : '(';
RPAREN : ')';
OPER: '==' | '<>' | '<' | '>' | '<=' | '>=';
OR : [Oo][Rr] | [��][��][��];
AND: [Aa][Nn][Dd] | [��];
NOT: [Nn][Oo][Tt] | [��][��];

NUMBER: DIGIT+;
IDENT: (LETTER | DIGIT)+;	
STRING : '\'' .*? '\'';