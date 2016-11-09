require "rest_client"
require "json"
FORCE_SERVER = '10.98.38.25:80'
def force_server
  RestClient::Resource.new(FORCE_SERVER, :headers=>{:accept => :json})
end
class AutomationTask 
  def AutomationTask.create_automation_task(params)
    response = force_server['tasks/'].post ARGV[0],:content_type => 'application/json'
    JSON.parse(response.body)
  end
end
AutomationTask.create_automation_task(ARGV)
