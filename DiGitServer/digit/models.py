from django.db import models
from _bsddb import version
from datetime import datetime

class User(models.Model):
    name            = models.CharField(max_length=50, null=True, blank=True)
    machine_name    = models.CharField(max_length=20)
    is_beta         = models.BooleanField(default=False)
    last_signal     = models.DateTimeField(default=datetime.now())
    version         = models.CharField(max_length=20)
    
    def __unicode__(self):
        return self.name if self.name else self.machine_name
           
    class Meta:
        app_label = "digit"
        
class UserVersion(models.Model):
    version         = models.CharField(max_length=20)
    date_installed  = models.DateTimeField()
    
    def __unicode__(self):
        return '{0} ({1})'.format(self.version, self.date_installed)
           
    class Meta:
        app_label = "digit"

    

class Activity(models.Model):
    user        = models.ForeignKey(User)
    activity    = models.CharField(max_length=1000)
    date_time   = models.DateTimeField('Date')
    error       = models.CharField(max_length=1000, null=True, blank=True)
    
    def __unicode__(self):
        return self.activity
           
    class Meta:
        app_label = "digit"