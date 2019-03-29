var btn = document.getElementsByName('submit');
var rightAnswers = new Array(2,0,1,1,2,0,2,0,2,0,0,0,0,0,0,0);
var userAnswers = new Array(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);
var judge = function () {
    var k=0;
    var score = 0;
    for(var i=1;i<=10;i++,k++)
    {
        var name="q";
        name+=i;
        var obj = document.getElementsByName(name);
        for(var j=0;j<obj.length;j++)
        {
            if(obj[j].checked==true && j==rightAnswers[i-1])
            {
                score+=10;
                userAnswers[k]=1;
            }
        }
    }
    return score;
    //console.log(score);
}
btn[0].addEventListener('click',function () {
    var boo = confirm("请确保所有题目已完成!");
    if(boo==true)
    {
        var score=judge();
        alert("您共答对："+(score/10)+"道题");
        for(var i=0;i<userAnswers.length;i++) {
            //console.log(userAnswers[i]);
            var ql = "q";
            var tem = i + 1;
            ql += tem;
            ql += rightAnswers[i];
            var q = document.getElementById(ql);
            if (userAnswers[i] == 0) {
                //console.log(ql);
                q.style.color = '#e10601';
            } else {
                q.style.color = '#000';
            }
            userAnswers[i]=0;
        }
    }
})
