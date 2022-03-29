# Rapport Arthur Soens


Pour lancer les trois projets, il faut lancer la solution td6 qui contient ClientSoapMathLibrary, MathLibrary et MathLibraryRest avec visual studio en mode admimistrateur.

### Projets :

__ClientSoapLibrary__: Ce projet est le client soap, son rôle est d'appeler le serveur soap et d'executer ses 4 fonctions (Add,Substarct,Multiply,Divide).

__MathLibrary__: Ce projet est le serveur soap, son rôle est de définir add, substract, multiply et divide. Chaque fonctions est appelé par le client soap défini plus haut.

__MathLibraryRest__: Ce projet est le serveur rest, son rôle est le même que MathLibrary à ceci près qu'il va être appelé en rest. Plus bas est defini un exemple d'url pour chaque appel.

### Exemple d'url pour l'appel au serveur rest :

__Add (1+2)__ :
http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Add?x=1&y=2

__Substract (1-2)__ :
http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Sub?x=1&y=2

__Multiply (2*4)__ :
http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Mult?x=2&y=4

__Divide (1/2)__ :
http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Div?x=1&y=2

__Divide with post request__ :

`POST http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/PostDiv`
Avec comme body : `{"x":3,"y":2}`