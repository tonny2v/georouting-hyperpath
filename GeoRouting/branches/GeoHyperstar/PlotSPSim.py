from matplotlib import pylab
import numpy as np
from matplotlib import rcParams
rcParams['font.family']='Times New Roman'
rcParams['font.size']=14
rcParams['lines.linewidth']=2
rcParams['figure.subplot.bottom']=0.10
rcParams['figure.subplot.left']=0.10
rcParams['figure.subplot.right']=0.95
rcParams['figure.subplot.top']=0.95

dtype=[('',np.double)]*10

with open('data.csv') as f:
    y=np.loadtxt(f,delimiter=',',dtype=dtype)

y=y.view(np.dtype([('data',np.double,10)]))
data=y['data']
print data.shape
pylab.boxplot(data,sym='+b')
pylab.xlabel('Test OD scenario ID',fontsize=16,family='Arial')
pylab.ylabel('Link inclusion ratio',fontsize=16,family='Arial')
pylab.ylim([0,1.1])
ax1=pylab.gca()
pos = np.arange(10)+1
data_new=range(10)
##medians=range(10)
means=range(10)
for i in range(10):
    print (data.reshape(10,10000)[i].shape)
    data_new[i]=data.reshape(10,10000).transpose()[i]

##    medians[i]=np.median(data_new[i])
    means[i]=np.mean(data_new[i])

upperLabels=[str(np.round(s, 2)) for s in means]

for tick,label in zip(range(10),ax1.get_xticklabels()):
   k = tick % 2
   ax1.text(pos[tick], 1.1-(1.1*0.05), upperLabels[tick],
        horizontalalignment='center', size=12,color='brown')

pylab.savefig('fig.tif',dpi=300,figsize=(4.81,3.64))
pylab.show()
