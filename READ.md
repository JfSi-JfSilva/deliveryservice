Todo o WebService funciona com json

Para ler o web config é necessário ler o HOST, se o HOST for trocado deverá, em cada um dos controladores alterar em conformidade onde está:

private string Host = "localhost";
A Autenticação é feita com base num token

existem as tabelas login e users, a tabela login tem o token e o apontador para a tabela users, 
a tabela users tem a informação do utilizador incluindo um identificador que diz se é ou não administador (is_admin)

na base de dados que é carregada pode-se verificar que o token do utilizador na tabela login é gravada 
com 'CONVERT(VARCHAR(256), HashBytes('SHA2_256', '0123456789 9876543210 ReD'), 2)','. 
O token para o utilizador tem de existir em determinados casos e é obrigatorio que seja um administrador para poder
efetuar a tarefa.

No web.config tem a string de ligação à base de dados que deverá de ser adapatada:

<connectionStrings>
<add
  name="DBConnectionString"
  connectionString="Data Source=JFSIDEVPC\SQLEXPRESS;Initial Catalog=dbdeliveryservice;Persist Security Info=True;User ID=delservice;Password=delivery"
  providerName="System.Data.SqlClient"
/>
</connectionStrings>

a base de dados foi acrescentada usando CLR e pode ser publicada com informação pre-definida para testes.

Pode ser necessário alterar a porta = 50881

Todos os testes foram efetuados com o POSTMAN.

Para descobrir o caminho mais rápido entre dois pontos tendo em conta o tempo

http://localhost:50881/api/Way

pasar a string json:

{
	"frompoint":"A",
	"topoint": "B",
	"sort_by": "time"
}

retorna uma string json:

"{\"frompoint\":\"A\",\"topoint\":\"B\",\"sort_by\":\"time\",\"path\":\"A->C->B\",\"err_message\":\"\"}"

Para descobrir o caminho mais rápido entre dois pontos tendo em conta o custo
http://localhost:50881/api/Way FAZENDO POST 
{
	"Frompoint":"A",
	"Topoint": "B",
	"Sort_by": "cost"
}

retorna uma string json:

"{\"frompoint\":\"A\",\"topoint\":\"B\",\"sort_by\":\"cost\",\"path\":\"A->H->E->D->F->I->B\",\"err_message\":\"\"}"

path: contem o melhor caminho.
err_message: possíveis erros que possam acontecer.

Para ver todas as rotas da base de dados com os respetivos pontos de passagem

http://localhost:50881/api/Route/GetAllRoute fazendo GET

retorna uma string json:

<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">
{"routes":[{"idroute":1,"route_code":"A-C-B","route_description":"A-C-B","waypoint":[{"fromidpoint":"A","toidpoint":"C","rp_cost":10000,"rp_time":1},{"fromidpoint":"C","toidpoint":"B","rp_cost":12,"rp_time":1}]},{"idroute":2,"route_code":"A-E-D","route_description":"A-E-D","waypoint":[{"fromidpoint":"A","toidpoint":"E","rp_cost":5,"rp_time":30},{"fromidpoint":"E","toidpoint":"D","rp_cost":5,"rp_time":3}]},{"idroute":3,"route_code":"D-F","route_description":"D-F","waypoint":[{"fromidpoint":"D","toidpoint":"F","rp_cost":50,"rp_time":5}]},{"idroute":4,"route_code":"F-I-B","route_description":"F-I-B","waypoint":[{"fromidpoint":"F","toidpoint":"I","rp_cost":50,"rp_time":45},{"fromidpoint":"I","toidpoint":"B","rp_cost":5,"rp_time":55}]},{"idroute":5,"route_code":"F-G-B","route_description":"F-G-B","waypoint":[{"fromidpoint":"F","toidpoint":"G","rp_cost":50,"rp_time":40},{"fromidpoint":"G","toidpoint":"B","rp_cost":73,"rp_time":64}]},{"idroute":6,"route_code":"A-H-E-D","route_description":"A-H-E-D","waypoint":[{"fromidpoint":"A","toidpoint":"H","rp_cost":1,"rp_time":10},{"fromidpoint":"H","toidpoint":"E","rp_cost":1,"rp_time":30},{"fromidpoint":"E","toidpoint":"D","rp_cost":5,"rp_time":3}]}],"err_message":"\r\n\r\n"}
</string>

para ver uma rota fasendo GET

http://localhost:50881/api/Route/1

retorna uma string json:

"{\"idroute\":1,\"route_code\":\"A-C-B\",\"route_description\":\"A-C-B\",\"waypoint\":[{\"fromidpoint\":\"A\",\"toidpoint\":\"C\",\"rp_cost\":10000,\"rp_time\":1},{\"fromidpoint\":\"C\",\"toidpoint\":\"B\",\"rp_cost\":12,\"rp_time\":1}],\"err_message\":\"\"}"

para eliminar uma rota

http://localhost:50881/api/Route/500 fazendo DELETE

Passar a string json 

{
	"Str_key": "8622846F4AD0EC8C3381172F8255AD82BA858132190A99076F5F2FA77A621948"
}

retorna uma string json:

"{\"ErrorMessage\":\"\\r\\n\\r\\n\\r\\n\\r\\n\\r\\nThere is no such route\\r\\n\"}"

para crair uma rota fazendo POST

http://localhost:50881/api/Route

passando no body uma string json com o seguinte formato

{
	"Str_key": "8622846F4AD0EC8C3381172F8255AD82BA858132190A99076F5F2FA77A621948",
	"Route_code": "Teste",
	"Route_description": "Teste",
	"Waypoint": [
		{
		"Rp_code": "T",
		"Rp_description": "T"
		},
		{
		"Rp_code": "Z",
		"Rp_description": "Z",
		"Rp_cost": 0,
		"Rp_time": 43
		}   
	]
}

Nota: pode indicar tantos Waypoint quando a rota tiver, sendo o primeiro ponto o inicio da rota e não tem custo ou tempo.

retorna uma string json:

"{\"ErrorMessage\":\"\\r\\nRoute created\\r\\n\"}"

para alterar uma rota fazendo PUT

http://localhost:50881/api/Route/7

passa os mesmos paraetros que na criacao a nível de body json.


Observacoes:

1.

Esta foi uma abordagem para resolver o problema, em alternativa, verificando-se que é necessário optimizar, pode-se
logo no momento em que criamos uma rota, colocar numa tabela todas as possibilizades de inicio e fim com uma adaptacao
da rotina GetWays. Desta forma, pesamos o insert, é verdade, no entanto optimizamos a forma como determinamos o melhor caminho
uma vez que bastaria um select.
Quase so era necessário alterar a primeira parte em que em vez de seleccionar um point pre-definido, selecionavamos todos 
os unicos fromidpoint. Armazenariamos tempo e custo para cada caminho como somatório.

2. 
Nao foi criado um controlador para os pontos, pode-se assim fazer caso seja necessário.
Foi pensado em que a criação dos pontos seria dinâmica aquando da criação da rota, i.e., quando cria a rota, cria
os pontos necessários.
