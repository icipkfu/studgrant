1. В папке Scripts npm install, bower install
2. Scripts\src\app\grant тут лежат все наработки по клиенту. Scripts\src\app\app.js есть константа API_CONFIG.serverPath в ней указывается путь сервера.
3. Для отладки я с F5 отдельно запускаю сервер и отдельно с gulp serve клиентскую часть. 
4. после gulp build необходимо взять файл Scripts\dist\index.html и закинуть его содержимое в Views\Home\Index.cshtml 
	изменив относительные пути подгружаемых скриптов. ну или просто изменить названия подргужаемых скриптов на актуальные.
4. F5/publish