from django.http import HttpResponse
from django.views.decorators.http import require_http_methods
from django.views.decorators.csrf import csrf_exempt
from django.utils import simplejson
from digit.models import *

@require_http_methods(["GET", "POST"])
@csrf_exempt
def add_event(request):
    json_str = request.body
    dct = simplejson.loads(json_str)
    user = None
    is_new_user = False
    if dct.has_key('UserInfo'):
        info = dct['UserInfo']
        user_machine = info['MachineName']
        ver = info['Version']
        
        user, is_new_user = User.objects.get_or_create(machine_name = user_machine)
        user.last_signal = datetime.now()
        
        
        user.save()
    else:
        return HttpResponse(False)
    
    if user:
        if dct.has_key('Activities'):
            for act in dct['Activities']:
                user_act = Activity()
                user_act.user = user
                user_act.activity = act['Activity']
                user_act.date_time = act['DateString'] # expected format: "yyyy-MM-dd HH:MM:ss"
                user_act.save()
    
    return HttpResponse(True)