# Web Api Authorize with JWT
# ASP Core

1 - git clone https://github.com/satem02/webapi-authorize.git  
2 - cd webapi-authorize  
3 - docker-compose -f "docker-compose.yml" up -d --build  
4 - http://localhost:5006/swagger/index.html  
5 - call /api/Auth/DummyLogin api and copy bearer token from response  
6 - add header Bearer $token  
7 - call /api/Values    
