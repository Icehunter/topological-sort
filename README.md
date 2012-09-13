# topological-sort

A simple implementation of topological sorting of dependencies first -> parents last.

This program was created as a work assigment and will be maintained as an alternative to matrix/vertex sorting.

## 'Interview' Test Request:

We have a number of items we want to place in order.
Each item can fall into one of four categories:
   - unnamed without dependencies
   - unnamed with dependencies
   - named without dependencies
   - named with dependencies
 
If an item is named other items that have dependencies can depend on it by name
 
Write a framework for defining items and a function that takes items on a given type
and return them in an order such that items with dependencies come later in the order
than the item or items they depend on.

## Results

I've presented two methods; and I'm open to advice and comment :) Topological takes about 2 minutes to sort 2500 things (it's quicker the smaller the dataset). Rabbit takes about .3 seconds for the same amount; this I custom coded myself.

I'd like someone to tell me if there's a better way; as I'm always looking to improve my code; and also tell me if I'm just doing it wrong :)

## Usage

After compiling the program create a sample.txt file in the bin/Release folder.

Add as many lines as you want into the file using the following format:

	parent:dep,dep,dep,...

* Open the program and press 'Import'.
* Press 'Save Dependency Tree' to sort dependencies first -> parents last, saving to a sorted CSV & copying it your clipboard. It will also save the original tree.

## Example sample.txt
	

	1:
	:
	testing:sath,e,parent2
	coolstamp:j,s,g,r,t,e,f
	:wuth,vike,kose
	:cill,poah,kijm,kose
	file:sath,pens
	j:parent2
	ders:kijm
	sath:kijm,kose,cill,coolstamp
	gyie:poah,file,ders
	pens
	wuth
	poah
	vike:kose,wert,pens
	kijm
	kose
	wert:cill,sath,tame
	cill
	tame:gyie
	d:e,f,g,h,i,j,l
	f:n,m,l,k
	m
	:f,g,h,j,i
	e:h,f,j,l
	c:g,j,m
	n
	b:f,g,m,n
	:f,e,a
	g:h,n,k
	h:i
	i
	j:really
	j:cool
	j:stuff
	e
	f:coolstamp
	a:c,d,h,j
	k
	l:1,
	:a,b,c,d
	:,1
	parent2: