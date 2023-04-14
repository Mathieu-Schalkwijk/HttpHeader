# HttpHeader

## Comment lancer le server
1. Ouvrez la solution NewWebRunner.sln dans le dossier NewWebRunner
2. Lancez "Program.cs"
3. Ouvrez un navigateur et allez à la page http://localhost:5000/
4. Lancez le scénario souhaité

## Comment fonctionne les requêtes et scénarios
L'interface graphique est une page web dynamique. En cliquant sur un des boutons, une requête est envoyée au serveur sur le port 5000 contenant le numéro du scénario à effectuer. Le serveur récupère ce numéro et va appeler la classe correspondante à ce scénario et ensuite renvoyé le résultat. Ce résultat sera ensuite affiché sur l'interface web.
