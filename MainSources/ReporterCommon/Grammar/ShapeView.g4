grammar ShapeView;

/* Parser Rules */

prog 	: statement (':' statement)*;

statement: IDENT '.' IDENT '=' expr      #StatementAction
		      | IDENT '=' expr                      #StatementVar
			  ;

expr : cons                                               #ExprCons		
        | STATE                                         #ExprState
		| IDENT										   #ExprVar
		| '(' expr ')'									   #ExprParen		
		| FUN '(' expr (';' expr)* ')'   		   #ExprFun
		| IF '(' (expr ';' expr ';')* expr ';' expr ';' expr ')'        #ExprIf
		| MINUS expr								   #ExprUnary				
		| expr OPER4 expr						   #ExprOper4		
		| expr (OPER3 | MINUS) expr         #ExprOper3		
		| expr OPER2 expr						   #ExprOper2		
		| NOT expr									   #ExprUnary
		| expr OPER1 expr							   #ExprOper1			
		;	

// онстанты
cons : INT                      #ConsInt
	   | REAL                    #ConsReal 
	   | STRING				  #ConsString	   
	   ;

/* Lexer Rules  */
//ѕробелы и комментарии
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

fragment OR : [Oo][Rr] | [»и][Ћл][»и];
fragment AND: [Aa][Nn][Dd] | [»и];
fragment XOR : [Xx][Oo][Rr] | [»и][—с][ к][Ћл][»и][Ћл][»и];
fragment MOD : [Mm][Oo][Dd];
fragment DIV : [Dd][Ii][Vv];
fragment BIT : [Bb][Ii][Tt] | [Ѕб][»и][“т];
fragment RGB : [Rr][Gg][Bb];
fragment ROUND : [Rr][Oo][Uu][Nn][Dd];

STATE : [Ss][Tt][Aa][Tt][Ee] | [—с][ќо][—с][“т];

IF : [Ii][Ff] | [≈е][—с][Ћл][»и];
NOT : [Nn][Oo][Tt] | [Ќн][≈е];
MINUS : '-';

OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=');
OPER1 : (AND | OR | XOR);

FUN : (BIT | RGB | ROUND);

ASSIGN : '=';

// онстанты  и  идентификаторы
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Zа-€ј-я];
fragment IDSYMB : (DIGIT | LETTER);

INT : DIGIT+;
REAL : INT ('.' | ',') INT
		 | INT (('.' | ',') INT) ? 'e' '-' ? INT
		 ;	

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
