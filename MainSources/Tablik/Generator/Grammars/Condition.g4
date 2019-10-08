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

fragment LETTER : [_a-zA-Z‡-ˇ¿-ﬂ];
fragment DIGIT : [0-9];

BTAG: '[';
ETAG: ']';

WS :  (' ' | '\t' | '\r' | '\n') -> skip;

PARENT : [Uu][Pp][Tt][Aa][Bb][Ll] | [ÕÌ][¿‡][ƒ‰][“Ú][¿‡][¡·][ÀÎ];
CHILDREN : [Ss][Uu][Bb][Tt][Aa][Bb][Ll] | [œÔ][ŒÓ][ƒ‰][“Ú][¿‡][¡·][ÀÎ];

LPAREN : '(';
RPAREN : ')';
OPER: '==' | '<>' | '<' | '>' | '<=' | '>=';
OR : [Oo][Rr] | [»Ë][ÀÎ][»Ë];
AND: [Aa][Nn][Dd] | [»Ë];
NOT: [Nn][Oo][Tt] | [ÕÌ][≈Â];

NUMBER: DIGIT+;
IDENT: (LETTER | DIGIT)+;	
STRING : '\'' .*? '\'';