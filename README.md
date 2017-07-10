# TB-SmartScreen - SCE - Scénario
## Système de gestion auto-organisé des employés d'une chaîne de production
### Problématique
Il n’est pas rare dans le domaine du travail à la chaîne que les employés aie la responsabilité de s’auto-organiser au niveau des éventuels remplacements dont ils devraient nécessité en cas d’absence. Ayant moi-même occupé un poste de ce type, j’ai été témoin de cette problématique organisationnelle. En effet, j’ai travaillé pendant plus d’une année en tant qu’opérateur horloger temporaire. J’étais payé à l’heure et mes périodes de travail occupaient généralement un ou deux jours par semaines ainsi que certains jours fériés et une partie de mes vacances.
Lors d’empêchement ne me permettant pas de travailler, il était de mon ressort de trouver quelqu’un voulant bien me remplacer. Je devais donc donner à chacun de mes collègues de travail les dates et les heures relatives à la période de mon empêchement. Cependant, généralement, aucun d’entre eux ne connaissait de mémoire leurs heures de travail. La plupart devaient donc sortir leurs horaires papier ou leurs agendas et corréler cela avec leurs emplois du temps. Également, certains aimaient bien calculer une estimation de leurs salaires pour déterminer si prendre des heures supplémentaires en valait vraiment la peine. Après plusieurs minutes d’attente, 80% des réponses étaient négatives. Pour cause : la majorité des gens travaillait déjà durant cette période. Lorsqu’un remplacement avait finalement été convenu, l’objectif était alors de trouver le responsable de chaîne (qui n’était pas toujours présent) et d’annoncer le remplacement. Celui-ci se pressait alors d’entrer manuellement la modification dans sa planification.
Il est important de mentionner que l’entreprise emploie également des personnes fixes. Dans un même contexte de remplacement, celles-ci ont en plus la contrainte de devoir recevoir le même nombre d’heures qu’elles donnent. Ceci, car elles doivent faire un nombre d’heures spécifiques par mois au risque de devoir prendre des jours de congé forcé pour avoir trop travaillé ou à l’inverse, devoir rattraper les heures qu’elles ont en retard. Sans parler des différentes qualifications de chaque employé, un employé qualifier pour travailler sur une certaine machine doit s’assurer d’échanger ces heures avec un employé aux mêmes qualifications.
### Besoins métier
Les besoins métier que j’ai identifié, découlant de cette problématique organisationnelle sont les suivants :
* Permettre aux employés d’avoir un accès facile et rapide à la consultation de leurs horaires de travail ;
* Permettre aux employés de consulter la liste des personnes disponible (ne travaillant pas) pour une période donnée ;
* Permettre aux employés d’avoir une estimation de leurs salaires à la fin du mois courant ;
* Permettre aux employés de donner ou de s’échanger équitablement leurs heures de travail en fonction :
  * De leurs types de contrats (temporaire ou fixe) ;
  * De leurs disponibilités ;
  * De leurs qualifications.
### Proposition de solution
La solution que je prépose pour répondre aux besoins métiers de la problématique est la mise en place d’un écran interactif au sein de l’entreprise. Cet écran, disponible à tous les collaborateurs de la chaîne de production, donnera accès à un système de gestion auto-organisé des employés.
Le principe de ce système est d’utiliser les interactions de notre catalogue (se référer au chapitre 2.4) pour rendre l’expérience utilisateur l'a plus intuitive et facile d’utilisation possible. Nous conviendrons également l’utilisation du capteur Kinect dans le cadre de son implémentation. L’utilisateur commence ainsi par s’identifier en présentant sa carte d’employé au capteur. S’il a accès au système, il sera redirigé instantanément sur un écran présentant un calendrier interactif dans lequel sont entrés ces horaires de travail. Le calendrier présente une vue par année, par mois et par semaines, l’utilisateur peut facilement naviguer entre ses vues grâce aux concepts d’interactions tactiles ou de navigation gestuelle supporter dans l’intégralité de l’application. Sur la vue par semaine, en sélectionnant une plage horaire, l’utilisateur voit apparaître la liste des employés à qui il peut potentiellement donner ou échanger ces heures. Sur un second écran, l’utilisateur peut consulter un certain nombre de statistiques liées à ces performances du mois courant. Ces statistiques sont les suivantes :
* Nombre d’heures comptabilisées ce mois ;
* Nombre de pièces comptabilisées ce mois ;
* Salaire par heures de travail ;
* Estimation salariale mensuelle.  

Lorsqu’une seconde personne se positionne devant l’affichage, l’idée est de lui permettre de s’identifier également et d’ainsi de leurs présenter ces deux écrans (calendrier et statistique) avec une mise en comparaison de leurs données respectives. Sur l’écran du calendrier, ils verront alors leurs horaires fusionnés et sur celui des statistiques, des graphiques simples présentant leurs statistiques personnelles. Étant maintenant deux devant l’affichage, en sélectionnant des plages horaires sur le calendrier, ils ont la possibilité de ce les données ou de réaliser un échange équitable dépendamment de leurs contrats de travail (fixe ou temporaire).
## Prototype fonctionnel
Dans l'état actuel, le prototype fonctionnel implémenter dans le cadre de cette thèse présente les fonctionnalités suivantes :
* Navigation gestuelle pour la sélection d'élément au sein de l'application ;
* Navigation par reconnaissace vocale ;
* Vue Calendrier
  * Vue par semaines ;
  * Mono-utilisateur
    * Consultation des heures de travail ;
  * Multiutilisateur
    * Fusion des heures de travail respectif ; 
    * Transfert d'heures de travail ;
* Vue Statistiques
  * Mono-utilisateur
    * Consultation des statistiques (à temps partiel) ;
  * Multiutilisateur
    * Comparatif des statistiques respectif ;
