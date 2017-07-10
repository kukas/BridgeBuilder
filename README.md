# BridgeBuilder
Bridge builder je hra/hříčka inspirovaná [Bridge Building Game](http://www.bridgebuilder-game.com/). Program simuluje fyziku mostů ve 2D prostředí, umožňuje mosty stavět a testovat s neomezenými konstrukčními možnostmi. Na rozdíl od své předlohy umožňuje stavbu bez zachycování do mřížky s cílem dovolit vytvářet organičtější a zajímavější návrhy.

## Spuštění
Program je vyvíjen v C#, pro prostředí .NET Framework 4.5.2

## Specifikace
Bridge builder je simulátor stavby mostů, program se skládá ze dvou hlavních částí - stavby a simulace.

Ve stavební části hráč může pomocí jednoduchého rozhraní umisťovat do prostoru pevné body (zastupující zemi), trámy (tedy dva propojené body) a vozovku (trám, na který lze ale aplikovat testovací zátěž). Jednotlivé body se zachycují do mřížky, pro jednodušší ovládání, nicméně tato funkce lze vypnout.
Po návrhu ve stavební části lze přepnout do simulační části, ve které lze myší interagovat s body mostu (aplikovat na ně zátěž), případně most "otestovat", čímž se rozumí nechat po mostu přejet zátěž, která se přesouvá zleva doprava po trámech typu vozovka.
Fyzikální simulace probíhá diskrétně, po časových úsecích, trámy jsou modelovány jako pružiny s vysokou tuhostí.
Při zvoleném pohledu "show stress" lze pozorovat intenzitu zátěže na jednotlivé trámy.
