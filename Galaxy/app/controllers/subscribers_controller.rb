class SubscribersController < ApplicationController
  before_action :set_subscriber, only: [:show, :edit, :update, :destroy]

  # GET /subscribers
  # GET /subscribers.json
  def index
    @subscribers = Subscriber.get_all_subscribers
  end

  # GET /subscribers/1
  # GET /subscribers/1.json
  def show
  end

  # GET /subscribers/new
  def new
    @subscriber = Subscriber.new
  end

  # GET /subscribers/1/edit
  def edit
    def @subscriber.new_record?()
        false
    end    
  end

  # POST /subscribers
  # POST /subscribers.json
  def create 
    temp_params = subscriber_params
    temp_params['create_time']="\/Date(#{Time.now.to_i}000)\/" 
    @subscriber = Subscriber.create_force_subscriber(temp_params)
    respond_to do |format|     
      if @subscriber
        format.html { redirect_to subscribers_path, notice: 'Subscriber was successfully created.' }
        format.json { render action: 'show', status: :created, location: @subscriber }
      else
        format.html { render action: 'new' }
        format.json { render json: @subscriber.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /subscribers/1
  # PATCH/PUT /subscribers/1.json
  def update
    respond_to do |format|
      if @subscriber.update(subscriber_params)
        format.html { redirect_to @subscriber, notice: 'Subscriber was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @subscriber.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /subscribers/1
  # DELETE /subscribers/1.json
  def destroy    
    Subscriber.delete_force_subscriber(params[:id])
    respond_to do |format|
      format.html { redirect_to subscribers_url }
      format.json { head :no_content }
      
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_subscriber
      @subscriber  = Subscriber.get_force_subscriber_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def subscriber_params
      params.require(:subscriber).permit(:project_id, :user_id, :create_time, :description, :subscriber_type)
    end
end
