from matplotlib import pylab
import numpy as np
from matplotlib import rcParams
rcParams['font.family']='Times New Roman'
rcParams['lines.linewidth']=2
dtype=[('',np.double)]*10

with open('..\..\data.csv') as f:
    y=np.loadtxt(f,delimiter=',',dtype=dtype)

y=y.view(np.dtype([('data',np.double,10)]))
data=y['data']
print data.shape
pylab.boxplot(data,sym='+b')
pylab.xlabel('Test OD scenario ID',fontsize=15,family='Arial')
pylab.ylabel('Link inclusion ratio',fontsize=15,family='Arial')
pylab.ylim([0,1.1])
ax1=pylab.gca()
pos = np.arange(10)+1
medians=range(10)
for i in range(10):
    print (data.reshape(10,10000)[i].shape)
    medians[i]=np.median(data.reshape(-1,1)[i])

upperLabels=[str(np.round(s, 2)) for s in medians]

for tick,label in zip(range(10),ax1.get_xticklabels()):
   k = tick % 2
   ax1.text(pos[tick], 1.1-(1.1*0.05), upperLabels[tick],
        horizontalalignment='center', size=10)
#pylab.show()
pylab.savefig('fig.png')
