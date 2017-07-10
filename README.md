# BridgeBuilder
Bridge builder je hra/hříčka inspirovaná [Bridge Building Game](http://www.bridgebuilder-game.com/). Program simuluje fyziku mostů ve 2D prostředí, umožňuje mosty stavět a testovat s neomezenými konstrukčními možnostmi. Na rozdíl od své předlohy umožňuje stavbu bez zachycování do mřížky s cílem dovolit vytvářet organičtější a zajímavější návrhy.

## Spuštění
Program je vyvíjen v jazyce C#, pro prostředí .NET Framework 4.5.2

## Specifikace
Bridge builder je simulátor stavby mostů, program se skládá ze dvou hlavních částí - stavby a simulace.

Ve stavební části hráč může pomocí jednoduchého rozhraní umisťovat do prostoru pevné body (zastupující zemi), trámy (tedy dva propojené body) a vozovku (trám, na který lze ale aplikovat testovací zátěž). Jednotlivé body se zachycují do mřížky, pro jednodušší ovládání, nicméně tato funkce lze vypnout.

Po návrhu ve stavební části lze přepnout do simulační části, ve které lze myší interagovat s body mostu (aplikovat na ně zátěž), případně most "otestovat", čímž se rozumí nechat po mostu přejet zátěž, která se přesouvá zleva doprava po trámech typu vozovka.

Fyzikální simulace probíhá diskrétně, po časových úsecích, trámy jsou modelovány jako pružiny s vysokou tuhostí.

Při zvoleném pohledu "show stress" lze pozorovat intenzitu zátěže na jednotlivé trámy.

## Ovládání
![screenshot aplikace](screenshots/screenshot.png)
V horní části okna se nachází simulace, v dolní pak nastavení rozdělená do kategorií, která ovlivňují chod programu.

### Popis  nastavení

**Simulation**

- _show stress_: Zapne vizualizaci napětí v trámech pomocí barev. (konkrétně zobrazuje, jak moc je daný trám natažený oproti své klidové délky).
- _pause simulation_: Zastaví simulaci, při spuštění programu je simulace zastavena.
- _gravitation_: Zapne působení gravitační síly v simulaci.
- _align to grid_: Zapne přichytávání do mřížky

**Testing**

- _speed_: Rychlost pohyblivého závaží.
- _weight_: Hmotnost pohyblivého závaží.
- _Run test_: Spustí pohyb závaží po vozovce mostu.

**Scene**

- _Load_: Načte uložený stav simulace.
- _Save_: Uloží aktuální stav simulace.
- _Clear_: Smaže vše z plochy simulace.

**Interaction**

- _place road_: Při zaškrtnutí lze místo umisťování trámů umisťovat vozovku. (také lze držet Shift při umisťování trámů)
- _fix vertex_: Při zaškrtnutí a kliknutí na bod v simulaci tento bod zafixuje.
- _add vertex_: Při zaškrtnutí a kliknutí kamkoli v simulaci umístí nový bod.

### První most
Po prvním spuštění bude plocha simulace prázdná. Pro umístění prvního bodu zaškrtněte checkbox _add vertex_ a klikněte kamkoli do černé plochy. Umístí se zde bod, ze kterého lze při stavbě vycházet. Odškrtněte _add vertex_, klikněte na přidaný bod a dalším klikáním mimo rozšiřujte vznikající most. Pro účely návodu vytvořte trojúhelník (levým tlačítkem lze vybrat existující body, a také nové přidávat, pravým tlačítkem zrušíte výběr). Nyní odškrtněte _pause simulation_ pro spuštění simulace, trojúhelník by měl spadnout k zemi. Při spuštěné simulaci nelze přidávat body/trámy, nicméně lze se simulací interagovat. Pomocí myši vezměte trojúhelník a pohybujte s ním po ploše.

Pro stavbu mostů je potřeba umístit do prostoru pevné body, které most bude spojovat. Zastavte tedy simulaci (zaškrtněte _pause simulation_), umístěte nový bod (zaškrtněte _add vertex_, umístěte bod, odškrtněte _add vertex_), umístěte 3 trámy vedle sebe (do přímky) a nyní zaškrtněte _fix vertex_, klikněte na oba krajní body této lávky (měl by se kolem nich objevit čtverec), odškrtněte _fix vertex_. Spusťte simulaci, lávka by měla držet mezi zafixovanými krajními body. Váš první most je hotový.

![první most](screenshots/first_bridge.png)

Pokud bychom chtěli most otestovat (tlačítko _Run test_), narazíme na problém, jelikož lávka se skládá pouze z trámů. Stiskněte tedy tlačítko _Clear_, opakujte předchozí postup vytváření lávky, nicméně při umisťování trámů zaškrtněte _place road_ (případně držte `Shift`). Nyní by se lávka měla skládat z dvojitých čar, které značí vozovku.

Před testováním je dobré si most uložit, klikněte na tlačítko _Save_, pro zajímavější pohled na simulaci zaškrtněte _show stress_ a pak klikněte na _Run test_. Testování probíhá přejezdem závaží (vizualizovaném kruhem, valícím se po vozovce). Zjistíme, že naše lávka se pod zátěží přetrhla.

Naštěstí jsme si most uložili, takže zastavíme aplikaci (_pause simulation_) a klikneme na _Load_. Pomocí trámů se pokuste zpevnit kontrukci mostu tak, aby vydržel zátěž _30000_. Pro inspiraci přikládám obrázek svého řešení.

![druhý most](screenshots/second_bridge.png)
