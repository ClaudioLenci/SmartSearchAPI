<style>
    .row_p
    {
        width: 6px;
        border-right: none;
        font-size: 15px;
    }
    
    .row_t
    {
        border-left: none;
    }
</style>

# SmartSearchAPI
## Scopo
Lo scopo di questa API è quello di semplificare la ricerca di documenti, testi e altre informazioni all'interno di applicazioni web. Il servizio deve comprendere il linguaggio naturale umano ed estrapolarne le informazioni chiave.

## Lingua
La lingua che ho impostato nel progetto è l'italiano. I file che contengono le informazioni necessarie sono:
<table border="solid">
    <tr>
        <td><kbd>./SmartSearchAPI/elastic_hunspell_master_dicts_it_IT.aff</kbd></td>
        <td rowspan="2">File di dizionario per Hunspell</td>
    </tr>
    <tr>
        <td><kbd>./SmartSearchAPI/elastic_hunspell_master_dicts_it_IT.dic</kbd></td>
    </tr>
    <tr>
        <td><kbd>./SmartSearchAPI/th_it_IT.dat</kbd></td>
        <td>File di lingua per Mythes</td>
    </tr>
</table>
Inoltre l'intelligenza artificiale Classifier è allenata con un dataset in italiano, dunque sarà necessario ricreare il dataset e riallenarla per cambiare lingua.

## Risultato
Il valore restituito dalla chiamata all'API è un json che rappresenta la classe SmartSearchResult. La classe (descritta qui sotto) è composta da una lista di SmartSearchKeyword e una lista di SmartSearchDateRange (anche queste classi sono descritte qui sotto).
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchResult</center></th></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Keywords&nbsp;&nbsp;:&nbsp;&nbsp;SmartSearchKeyword [0 .. *]</td>
    </tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateRanges&nbsp;&nbsp;:&nbsp;&nbsp;SmartSearchDateRange [0 .. *]</td>
    </tr>
</table>
&nbsp;
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchKeyword</center></th></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Noun&nbsp;&nbsp;:&nbsp;&nbsp;string</td>
    </tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Synonyms&nbsp;&nbsp;:&nbsp;&nbsp;string [0 .. *]</td>
    </tr>
</table>
&nbsp;
<table border="solid" width="340px">
    <tr><th colspan="2"><center>SmartSearchDateRange</center></th></tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateMin&nbsp;&nbsp;:&nbsp;&nbsp;DateTime</td>
    </tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">DateMax&nbsp;&nbsp;:&nbsp;&nbsp;DateTime</td>
    </tr>
    <tr>
        <td class="row_p">-</td>
        <td class="row_t">Include&nbsp;&nbsp;:&nbsp;&nbsp;bool</td>
    </tr>
</table>

## Considerazioni finali