<link href="readme_style.css" rel="stylesheet">

# SmartSearchAPI
## Scopo
Lo scopo di questa API è quello di semplificare la ricerca di documenti, testi e altre informazioni all'interno di applicazioni web. Il servizio deve comprendere il linguaggio naturale umano ed estrapolarne le informazioni chiave.

## Lingua
La lingua che ho impostato nel progetto è l'italiano. I file che contengono le informazioni necessarie sono:
<table border="solid">
    <tr>
        <td><kbd>./SmartSearchAPI/elastic_hunspell_master_dicts_it_IT.aff</kbd></td>
        <td rowspan="3">File di dizionario per Hunspell</td>
        <td rowspan="3">Scarica i file <a href="https://github.com/wooorm/dictionaries/">qui</a> oppure <a href="https://github.com/elastic/hunspell/">qui</a></td>
    </tr>
    <tr></tr>
    <tr>
        <td><kbd>./SmartSearchAPI/elastic_hunspell_master_dicts_it_IT.dic</kbd></td>
    </tr>
    <tr></tr>
    <tr>
        <td><kbd>./SmartSearchAPI/th_it_IT.dat</kbd></td>
        <td>File di lingua per Mythes</td>
        <td>Scarica i file <a href="https://extensions.openoffice.org/en/search?f%5B0%5D=field_project_tags%3A157">qui</a> oppure <a href="https://wiki.openoffice.org/wiki/Dictionaries">qui</a></td>
    </tr>
</table>

Inoltre l'intelligenza artificiale che l'API utilizza per fare l'analisi grammaticale, Catalyst, è impostata in italiano. Per selezionare l'italiano ho dovuto installare il pacchetto <kbd>Catalyst.Models.Italian</kbd> e adattare una parte di codice nella classe NLP_processor. Quindi per cambiare la lingua sarà necessario:
<ol>
    <li>Installare il pacchetto <kbd>Catalyst.Models.-lingua-</kbd></li>
    <li>Modificare il seguente pezzo di codice nella classe NLP_processor:<br>
        ```Catalyst.Models.Italian.Register();<br> Storage.Current = new DiskStorage("catalyst-models");<br>var nlp = await Pipeline.ForAsync(Language.Italian);<br>var doc = new Document(input, Language.Italian);```
    </li>
</ol>

Infine l'intelligenza artificiale Classifier è allenata con un dataset in italiano, dunque sarà necessario ricreare il dataset e riallenarla per cambiare lingua.

## Risultato
Il valore restituito dalla chiamata all'API è un json che rappresenta la classe SmartSearchResult. La classe (descritta qui sotto) è composta da una lista di SmartSearchKeyword e una lista di SmartSearchDateRange (anche queste classi sono descritte qui sotto).
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchResult</center></th></tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Keywords&nbsp;&nbsp;:&nbsp;&nbsp;SmartSearchKeyword [0 .. *]</td>
    </tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateRanges&nbsp;&nbsp;:&nbsp;&nbsp;SmartSearchDateRange [0 .. *]</td>
    </tr>
</table>
&nbsp;
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchKeyword</center></th></tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Noun&nbsp;&nbsp;:&nbsp;&nbsp;string</td>
    </tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Synonyms&nbsp;&nbsp;:&nbsp;&nbsp;string [0 .. *]</td>
    </tr>
</table>
&nbsp;
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchDateRange</center></th></tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateMin&nbsp;&nbsp;:&nbsp;&nbsp;DateTime</td>
    </tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateMax&nbsp;&nbsp;:&nbsp;&nbsp;DateTime</td>
    </tr>
    <tr></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Include&nbsp;&nbsp;:&nbsp;&nbsp;bool</td>
    </tr>
</table>

## Considerazioni finali