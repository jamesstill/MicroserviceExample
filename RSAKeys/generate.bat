C:\OpenSSL-Win64\bin\openssl genrsa -out private.pem 2048 
C:\OpenSSL-Win64\bin\openssl rsa -in private.pem -outform PEM -pubout -out public.pem