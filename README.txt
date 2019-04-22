				Fourplaces


Conseil pour tester l'application:


Si la compilation ne fonctionne pas sur Xamarin:

Si Mono.Android est présent dans les dépendances de Fourplaces, il est nécessaire de le supprimer.
Il est peut-être aussi nécessaire de mettre à jour les références de Fourplaces.Android.

Possibilité d'installer l'apk de l'app.
Mais si on veut la supprimer puis la réinstaller, il faudra probablement désactiver google play protect.


Points importants:


Toutes les requêtes fonctionnent correctement.
Les erreurs faîtes par l'utilisateur ou à cause d'un problème de connexion ou de la géolocalisation sont gérées.

Pour la gestion du token:
Tant que l'application n'est pas fermée, à chaque fois qu'il est nécessaire d'utiliser le token, une vérification est faite pour savoir si le token a expiré ou non. Si c'est le cas un refresh du token est effectué automatiquement.
Lorsque l'on relance l'application, grâce au cache , l'utilisateur s'il était connecté lors de sa dernière session, est automatiquement connecté grâce à la sauvegarde en cache(sauvegardé durant 1 jour) de son token.
Pour qu'il n'y est pas de problème de token expiré, un refresh du token est fait lors de la première requête demandant un token.

Le système de cache est utilisé lorsqu'on est pas connecté.
Il est également utilisé pour les images des lieux dans la liste view du menu principal(car on ne peut pas modifier l'image d'un lieu).
Cela évite à l'utilisateur de re-télécharger en permanence les images des lieux.

Si la géolocalisation n'est pas activée, par défaut la liste des lieux s'affichera dans leur ordre de création.

Pour refresh la liste de lieu, il est nécessaire de passer sur une autre page puis de revenir sur la liste.

Après être connecté, inscrit ou avoir ajouté un lieu, on est directement redirigé vers la
Liste de lieu rafraichie. 

Lorsqu'on veut ajouter un lieu et que la géolocalisation est activé , les champs latitude et longitude seront remplacés par la latitude et la longitude actuelle de l'utilisateur.


Problèmes rencontrés:


Lorsqu'on lance l'application pour la première fois, on nous demande d'autoriser la géolocalisation. Mais au moment d'arriver sur la liste view, l'app n'a pas le temps de récupérer la géolocalisation et affiche les lieux dans leur ordre de création.

Même si le cache fonctionne, il y a souvent lorsqu'on scroll dans la liste view des lags plus ou moins important. 

Il arrive aussi quelques fois que l'app crash à cause de dépassement de la mémoire lié probablement au système de cache.


Non fait:
Rendre l'application responsive
Retravailler l'esthétique de l'application
Permettre d'utiliser l'application dans plusieurs langues

















