# -*- coding: utf-8 -*-
import datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding model 'User'
        db.create_table(u'digit_user', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=50, null=True, blank=True)),
            ('machine_name', self.gf('django.db.models.fields.CharField')(max_length=20)),
            ('is_beta', self.gf('django.db.models.fields.BooleanField')(default=False)),
            ('last_signal', self.gf('django.db.models.fields.DateTimeField')(default=datetime.datetime(2015, 12, 30, 0, 0))),
            ('version', self.gf('django.db.models.fields.CharField')(max_length=20)),
        ))
        db.send_create_signal('digit', ['User'])

        # Adding model 'UserVersion'
        db.create_table(u'digit_userversion', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('version', self.gf('django.db.models.fields.CharField')(max_length=20)),
            ('date_installed', self.gf('django.db.models.fields.DateTimeField')()),
        ))
        db.send_create_signal('digit', ['UserVersion'])

        # Adding model 'Activity'
        db.create_table(u'digit_activity', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('user', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['digit.User'])),
            ('activity', self.gf('django.db.models.fields.CharField')(max_length=1000)),
            ('date_time', self.gf('django.db.models.fields.DateTimeField')()),
            ('error', self.gf('django.db.models.fields.CharField')(max_length=1000, null=True, blank=True)),
        ))
        db.send_create_signal('digit', ['Activity'])


    def backwards(self, orm):
        # Deleting model 'User'
        db.delete_table(u'digit_user')

        # Deleting model 'UserVersion'
        db.delete_table(u'digit_userversion')

        # Deleting model 'Activity'
        db.delete_table(u'digit_activity')


    models = {
        'digit.activity': {
            'Meta': {'object_name': 'Activity'},
            'activity': ('django.db.models.fields.CharField', [], {'max_length': '1000'}),
            'date_time': ('django.db.models.fields.DateTimeField', [], {}),
            'error': ('django.db.models.fields.CharField', [], {'max_length': '1000', 'null': 'True', 'blank': 'True'}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'user': ('django.db.models.fields.related.ForeignKey', [], {'to': "orm['digit.User']"})
        },
        'digit.user': {
            'Meta': {'object_name': 'User'},
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'is_beta': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'last_signal': ('django.db.models.fields.DateTimeField', [], {'default': 'datetime.datetime(2015, 12, 30, 0, 0)'}),
            'machine_name': ('django.db.models.fields.CharField', [], {'max_length': '20'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '50', 'null': 'True', 'blank': 'True'}),
            'version': ('django.db.models.fields.CharField', [], {'max_length': '20'})
        },
        'digit.userversion': {
            'Meta': {'object_name': 'UserVersion'},
            'date_installed': ('django.db.models.fields.DateTimeField', [], {}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'version': ('django.db.models.fields.CharField', [], {'max_length': '20'})
        }
    }

    complete_apps = ['digit']