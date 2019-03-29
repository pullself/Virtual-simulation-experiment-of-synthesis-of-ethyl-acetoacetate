var preBtn = document.getElementsByClassName('preBtn');
var nextBtn = document.getElementsByClassName('nextBtn');
var pages = document.getElementsByClassName('page');
var content = document.getElementsByClassName('content');
var index = 0;
var clearActive = function () {
    for(var i=0;i<pages.length;i++)
    {
        pages[i].className='page';
    }
}
var goIndex = function () {
    clearActive();
    pages[index].className = 'page active';
    if(index>0)
    {
        content[0].className='content';
        content[1].className= 'content active';
    }
    else
    {
        content[0].className='content active';
        content[1].className= 'content';
    }
}

var goNext = function () {
    console.log(index);
    if(index<6)
        index++;
    goIndex();
}
var goPre = function(){
    if(index!=0)
        index--;
    goIndex();
}
preBtn[0].addEventListener('click',function () {
    goPre();
})
nextBtn[0].addEventListener('click',function () {
    goNext();
})
for(var i=0;i<pages.length;i++)
{
    pages[i].addEventListener('click',function () {
        index = this.getAttribute('data-index');
        goIndex();
    })
}
