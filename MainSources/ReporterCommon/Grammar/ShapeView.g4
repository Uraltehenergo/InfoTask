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

//���������
cons : INT                      #ConsInt
	   | REAL                    #ConsReal 
	   | STRING				  #ConsString	   
	   ;

/* Lexer Rules  */
//������� � �����������
WS  : [ \n\r\t] -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT : '//' .*? '\r'? '\n' -> skip;

fragment OR : [Oo][Rr] | [��][��][��];
fragment AND: [Aa][Nn][Dd] | [��];
fragment XOR : [Xx][Oo][Rr] | [��][��][��][��][��][��][��];
fragment MOD : [Mm][Oo][Dd];
fragment DIV : [Dd][Ii][Vv];
fragment BIT : [Bb][Ii][Tt] | [��][��][��];
fragment RGB : [Rr][Gg][Bb];
fragment ROUND : [Rr][Oo][Uu][Nn][Dd];

STATE : [Ss][Tt][Aa][Tt][Ee] | [��][��][��][��];

IF : [Ii][Ff] | [��][��][��][��];
NOT : [Nn][Oo][Tt] | [��][��];
MINUS : '-';

OPER4 : ('*' | '/' | DIV | MOD);
OPER3 : ('+' );
OPER2 : ('==' | '<>' | '<' | '>' | '<=' | '>=');
OPER1 : (AND | OR | XOR);

FUN : (BIT | RGB | ROUND);

ASSIGN : '=';

//���������  �  ��������������
fragment DIGIT : [0-9];
fragment LETTER : [_a-zA-Z�-��-�];
fragment IDSYMB : (DIGIT | LETTER);

INT : DIGIT+;
REAL : INT ('.' | ',') INT
		 | INT (('.' | ',') INT) ? 'e' '-' ? INT
		 ;	

IDENT : IDSYMB* LETTER IDSYMB*;
STRING : '\'' ('\'\'' | ~[\'])*? '\'';
