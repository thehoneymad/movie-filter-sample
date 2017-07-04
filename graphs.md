<img src='https://g.gravizo.com/svg?
 digraph G {
   Ted -> Barney[label="friend"];
   Barney -> Robin[label="wife"];
   Robin -> Barney[label="husband"];
   Umbrella [shape=box];
   Ted -> Umbrella[label="found"];
   Tracy -> Umbrella[label="lost"];
   Ted -> Tracy[label="wife"];
   Barney -> Marshall[label="friend"];
   Ted -> Marshall[label="friend"];
   Ted -> Robin[label="friend"];
   MacLarensPub[shape="box"];
   Marshall -> MacLarensPub[label="hangs out"];
   Ted -> MacLarensPub[label="hangs out"];
   Barney -> MacLarensPub[label="hangs out"];
   Robin -> MacLarensPub[label="hangs out"];
 }
'/>

<img src='https://g.gravizo.com/svg?
 digraph G {
   movie [shape=box]
   user -> movie [label="rates: 4.5"]
 }
'/>

<img src='https://g.gravizo.com/svg?
 digraph G {
   movie [shape=box];
   user -> movie1 [label="rates: 4.5"];
   user -> movie2 [label="rates: 5"];
   user -> movie3 [label="rates: 5"];
 }
'/>

<img src='https://g.gravizo.com/svg?
 digraph G {
   movie1 [shape=box];
   movie2 [shape=box];
   movie3 [shape=box];
   user -> movie1 [label="rates: 4.5"];
   user -> movie2 [label="rates: 5"];
   user -> movie3 [label="rates: 5"];
   user2 -> movie2 [label="rates: 4.5"];
   user2 -> movie3 [label="rates: 5"];
   user3 -> movie1 [label="rates: 4.5"];
   user3 -> movie2 [label="rates: 5"];
 }
'/>

<img src='https://g.gravizo.com/svg?
 digraph G {
   movie1 [shape=box];
   movie2 [shape=box];
   movie3 [shape=box];
   movie4 [shape=box];
   movie5 [shape=box];
   user -> movie1 [label="rates: 4.5"];
   user -> movie2 [label="rates: 5"];
   user -> movie3 [label="rates: 5"];
   user2 -> movie2 [label="rates: 4.5"];
   user2 -> movie3 [label="rates: 5"];
   user3 -> movie1 [label="rates: 4.5"];
   user3 -> movie2 [label="rates: 5"];
   user3 -> movie4 [label="rates: 5"];
   user2 -> movie5 [label="rates: 5"];
 }
'/>

