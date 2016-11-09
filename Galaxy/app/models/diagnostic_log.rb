class DiagnosticLog < ActiveRecord::Base
private
  def DiagnosticLog.initialize_from_json_params(params)
    diagnosticLog = DiagnosticLog.new    
    diagnosticLog.id = params['LogId']
    diagnosticLog.component = params['Component']
    diagnosticLog.log_type = params['LogType']
    diagnosticLog.create_time = params['CreateTime']
    diagnosticLog.message = params['Message']
    diagnosticLog
  end
  def DiagnosticLog.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Component] = params['component']
    hash[:LogType] = params['log_type']
    hash[:CreateTime] = params['create_time']
    hash[:Message] = params['message']
    JSON.generate(hash)
  end
end
