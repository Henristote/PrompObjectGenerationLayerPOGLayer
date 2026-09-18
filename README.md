# Prompt Object Generation Layer (POGLayer)

Ce projet Unity est une expérience VR permettant la génération de modèles 3D directement via un prompt vocal. Concrètement, l'utilisateur dicte à voix haute l'élément qu'il souhaite faire apparaître, le système génère le modèle correspondant, et cet objet devient immédiatement manipulable au sein de l'environnement virtuel. L'idée est de rendre la matérialisation et l'interaction spatiale beaucoup plus intuitives, sans passer par des interfaces lourdes. Depuis l'éditeur les éléments sont stockés dans le dossiers Assets/Models.

Concernant la mise en place de l'environnement de travail, la structure de ce dépôt repose sur l'utilisation de sous-modules Git. Cela implique qu'un téléchargement classique de l'archive ou un simple clonage de base ne rapatriera pas tout le code nécessaire à la bonne exécution du projet.

Pour obtenir l'intégralité des fichiers dès le départ, il est nécessaire d'utiliser la ligne de commande pour cloner le projet en incluant directement ses dépendances. Placez-vous dans le dossier de destination de votre choix et exécutez la commande suivante :

```bash
git clone --recursive https://github.com/Henristote/PrompObjectGenerationLayerPOGLayer.git
```

Dans le cas où vous auriez déjà effectué un clonage standard par habitude, l'oubli est facilement rattrapable. Ouvrez votre terminal, naviguez jusqu'à la racine du projet que vous venez de récupérer, puis lancez l'instruction de mise à jour des sous-modules :

```bash
git submodule update --init --recursive
```

Cette action se chargera d'inspecter l'arborescence, d'initialiser les liens manquants et de télécharger les composants requis.

Une fois que tous les fichiers sont bien présents sur votre machine, il ne vous reste plus qu'à ajouter le dossier via le Unity Hub et à ouvrir le projet. Vous pourrez alors tester la chaîne complète, de la captation de votre voix jusqu'à la manipulation physique de l'objet 3D généré dans votre scène.
